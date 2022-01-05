using System;
using Npgsql;
using OR_Mapper.App.ObjectClasses;
using OR_Mapper.Framework;
using OR_Mapper.Framework.Database;

namespace OR_Mapper.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Db.Connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=postgres;Database=postgres");

            //Db.Query("INSERT INTO swe3_orm.person (id, name) VALUES (10, 'seppi')");
            
            // Generate new object
            var myTeacher = new Teacher();
            myTeacher.Id = 101;
            myTeacher.FirstName = "Fred";
            myTeacher.Name = "Keks";
            myTeacher.Salary = 1000;

            var myClass = new Class
            {
                Id = 1,
                Name = "MyClass",
                Teacher = myTeacher
            };
            
            // Save Object
            myClass.Save();
            //myTeacher.Save();



            //var sql = "INSERT INTO swe3_orm.person (id, name) VALUES (20, 'hans')";
            

            var model = new Model(typeof(Student));

            Console.WriteLine();
            
        }
    }
}
