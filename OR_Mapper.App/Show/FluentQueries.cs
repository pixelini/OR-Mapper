using System;
using System.Linq;
using Npgsql;
using OR_Mapper.App.ObjectClasses;
using OR_Mapper.Framework.Database;
using OR_Mapper.Framework.FluentApi;

namespace OR_Mapper.App.Show
{
    public class FluentQueries
    {
        public static void Show()
        {
            Console.WriteLine();
            Console.WriteLine("7] Fluent Queries");
            Console.WriteLine("---------------------------");
            
            FluentApi.UseConnection(() => new NpgsqlConnection(Db.ConnectionString));
            var result1 = FluentApi.Entity<Teacher>().Where("Name").Is("Hansson").Execute();
            Console.WriteLine($"Result 1: {result1[0].Name}");

            var result2 = FluentApi.Entity<Teacher>().Where("Name").Is("Hansson").And("Salary").Is(100000).Execute();
            Console.WriteLine($"Result 2: {result2[0].Name}, Salary: {result2[0].Salary}");

            var result3 = FluentApi.Entity<Teacher>().Max("Salary").Execute();
            Console.WriteLine($"Result 3: Highest teacher salary: {result3}");

            var result4 = FluentApi.Entity<Teacher>().Where("Name").Is("Hansson").And("Salary").IsGreaterThan(20).Execute();
            Console.WriteLine($"Result 4: {result4[0].Name}, Salary: {result4[0].Salary}");
        }
    }
}