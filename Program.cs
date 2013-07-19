using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Directory = Lucene.Net.Store.Directory;
using Version = Lucene.Net.Util.Version;

namespace ConsoleApplication7
{
    class Program
    {
        static void Main(string[] args)
        {
            SearchIndex index = new SearchIndex();
            Student student1 = new Student
                {
                    Id = 111,
                    Name = "Jack",
                    Address = "Shinetech Xi'an",
                    Description = "Shinetech Developer"
                };

            Student student2 = new Student
            {
                Id = 112,
                Name = "Jack",
                Address = "Shinetech Xi'an",
                Description = "Shinetech Developer"
            };
            Student student3 = new Student
            {
                Id = 113,
                Name = "Jack",
                Address = "IBM Beijing",
                Description = "Shinetech Developer"
            };
            Student student4 = new Student
            {
                Id = 114,
                Name = "Hulu",
                Address = "Shinetech Beijing",
                Description = "IBM Tester"
            };
            Student student5 = new Student
            {
                Id = 115,
                Name = "Hulu",
                Address = "IBM Xi'an",
                Description = "Shinetech Tester"
            };
            index.AddIndex(student1);
            index.AddIndex(student2);
            index.AddIndex(student3);
            index.AddIndex(student4);
            index.AddIndex(student5);

            SearchSearcher searcher = new SearchSearcher();
          //  var results = searcher.Search("");

            var results = searcher.GetByIds();

        }
    }


    public class Student
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
    }


    public class SearchIndex
    {
        public void AddIndex(Student student)
        {
            Directory directory = FSDirectory.Open(new DirectoryInfo(@"D:\TestIndex"));

            using (Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30))
            {
                using (var writer = new IndexWriter(directory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED))
                {
                    var doc = new Document();
                    doc.Add(new Field("Id", student.Id.ToString(), Field.Store.YES, Field.Index.ANALYZED));
                    doc.Add(new Field("Name", student.Name, Field.Store.YES, Field.Index.ANALYZED));
                    doc.Add(new Field("Description", student.Description, Field.Store.YES, Field.Index.ANALYZED));
                    doc.Add(new Field("Address", student.Address, Field.Store.YES, Field.Index.ANALYZED));

                    if (new SearchSearcher().GetById(student.Id) != null) return;
                    writer.AddDocument(doc);
                    writer.Commit();
                }
            }
        }
    }


    public class SearchSearcher
    {
        public List<Student> Search(string text)
        {

            List<Student> students = new List<Student>();

            Directory directory = FSDirectory.Open(new DirectoryInfo(@"D:\TestIndex"));

            using (Analyzer analyzer = new StandardAnalyzer(Version.LUCENE_30))
            {
                MultiFieldQueryParser parser = new MultiFieldQueryParser(Version.LUCENE_30, new[] { "Id", "Name", "Description", "Address" }, analyzer);

                Query query = parser.Parse(text);

                using (var searcher = new IndexSearcher(directory, true))
                {
                    TopDocs topDocs = searcher.Search(query, 20);

                    foreach (var item in topDocs.ScoreDocs)
                    {
                        Document doc = searcher.Doc(item.Doc);
                        students.Add(new Student
                        {
                            Id = long.Parse(doc.Get("Id")),
                            Name = doc.Get("Name"),
                            Description = doc.Get("Description"),
                            Address = doc.Get("Address")
                        });
                    }

                }

            }

            return students;
        }

        public Student GetById(long id)
        {

            Student student = null;

            Directory directory = FSDirectory.Open(new DirectoryInfo(@"D:\TestIndex"));
            
            TermQuery query = new TermQuery(new Term("Id", id.ToString()));

            using (var searcher = new IndexSearcher(directory, true))
            {

                TopDocs topDocs = searcher.Search(query, 20);

                if (topDocs.ScoreDocs.Count() == 1)
                {
                    Document doc = searcher.Doc(topDocs.ScoreDocs[0].Doc);

                    student = new Student
                        {
                            Id = long.Parse(doc.Get("Id")),
                            Name = doc.Get("Name"),
                            Description = doc.Get("Description"),
                            Address = doc.Get("Address")
                        };
                }
            }
            return student;
        }


        public List<Student> GetByIds()
        {

            Student student = null;

            Directory directory = FSDirectory.Open(new DirectoryInfo(@"D:\TestIndex"));


            MatchAllDocsQuery query=new MatchAllDocsQuery("Id");

            
            

            using (var searcher = new IndexSearcher(directory, true))
            {

                TopDocs topDocs = searcher.Search(query, 2);

                if (topDocs.ScoreDocs.Count() == 1)
                {
                    Document doc = searcher.Doc(topDocs.ScoreDocs[0].Doc);

                    student = new Student
                    {
                        Id = long.Parse(doc.Get("Id")),
                        Name = doc.Get("Name"),
                        Description = doc.Get("Description"),
                        Address = doc.Get("Address")
                    };
                }
            }
            return null;
        }

    }
}
