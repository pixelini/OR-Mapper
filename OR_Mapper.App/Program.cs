using System;
using System.Collections.Generic;
using System.Linq;
using Npgsql;
using OR_Mapper.App.ObjectClasses;
using OR_Mapper.App.Show;
using OR_Mapper.Framework.Database;
using OR_Mapper.Framework.FluentApi;

namespace OR_Mapper.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Configure database connection
            Db.DbSchema = "swe3_orm";
            Db.ConnectionString = "Host=localhost;Username=postgres;Password=postgres;Database=postgres";

            var cache = Db.Cache.SingleCache;
            Console.WriteLine($"Cache count: {cache.Count}");
            
            InsertObject.Show();
            EditObject.Show();
            DeleteObject.Show();
            WithFk.Show();
            WithFkList.Show();
            LazyLoading.Show();
            FluentQueries.Show();
            
            Console.WriteLine($"Cache count: {cache.Count}"); // Count 2: means 2 caches from entities Teacher and Class
            Console.WriteLine();
        }
    }
}
