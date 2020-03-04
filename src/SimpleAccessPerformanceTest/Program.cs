using System;
using System.Diagnostics;
using System.Net;
using Dapper;
using Microsoft.Data.SqlClient;
using SimpleAccess;
using SimpleAccess.SqlServer;

namespace SimpleAccessPerformanceTest
{
    class Program
    {
        private static string ConnectionString = "Data Source=.\\SQLEXPRESS2017;Initial Catalog=SimpleAccessTest;Persist Security Info=True;User ID=sa;Password=Test123;";
        static void Main(string[] args)
        {

            //CreateRecord();
            using (var connection = new SqlConnection(ConnectionString)) { }

            var stopWatch = new Stopwatch();

            Console.WriteLine("Starting Dapper...");
            stopWatch.Start();
            RunDapper();
            stopWatch.Stop();
            Console.WriteLine("Total Dapper Time: {0}", stopWatch.ElapsedMilliseconds);

            Console.WriteLine("Starting SimpleAccess...");

            stopWatch.Restart();
            RunSimpleAccess();
            stopWatch.Stop();
            Console.WriteLine("Total SimpleAccess Time: {0}", stopWatch.ElapsedMilliseconds);

            Console.ReadKey();
        }

        static void RunDapper()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var query = "SELECT ID, CityId, Name, PhoneNumbers,Address ,Address2  FROM  BRANCHES";

                for (int i = 0; i < 500; i++)
                {
                    var branch = connection.QueryFirst<Branch>(query);

                    var b = branch != null;
                }
            }
        }

        public static void RunSimpleAccess()
        {
            var simpleAccess = new SqlSimpleAccess(ConnectionString);
            var query = "SELECT ID, CityId, Name, PhoneNumbers,Address ,Address2  FROM  BRANCHES";

            for (int i = 0; i < 500; i++)
            {
                var branch = simpleAccess.ExecuteEntity<Branch>(query);

                var b = branch != null;
            }
        }

        static void CreateRecord()
        {
            using (var connection = new SqlConnection(ConnectionString))
            {
                var query =
                    "INSERT INTO BRANCHES (Id, CityId, Name, PhoneNumbers,Address ,Address2) VALUES (1, @CityId, @Name, @PhoneNumbers, @Address , @Address2)";

                for (int i = 0; i < 1; i++)
                {
                    connection.Execute(query, new Branch
                    {
                        CityId = i,
                        Address = "Address " + i,
                        Address2 = "Address " + i,
                        Name = "Branch " + i,
                        PhoneNumbers = "234234 23234"
                    });
                }
            }
        }

    }

    [Entity("Branches")]
    public class Branch
    {
        [Identity]
        public int Id { get; set; }

        public int CityId { get; set; }
        public string Name { get; set; }
        public string PhoneNumbers { get; set; }
        public string Address { get; set; }

        public string Address2 { get; set; }
    }
}
