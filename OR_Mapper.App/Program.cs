using System;
using OR_Mapper.App.ObjectClasses;
using OR_Mapper.Framework;

namespace OR_Mapper.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            var model = new Model(typeof(Teacher));
            Console.WriteLine();


            /*// config
            Db.Connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=postgres;Database=postgres");
            Db.Connection.Open();
            //var result = Db.Query("INSERT INTO swe3_orm.person (id, name) VALUES (1, 'hans')");
            var result = Db.Fetch("select * from swe3_orm.person");

            Console.WriteLine(result);*/



        }
    }
}
