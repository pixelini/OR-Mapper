using System;
using System.Collections.Generic;
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
            Db.DbSchema = "swe3_orm";
            Db.Connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=postgres;Database=postgres");

            //Db.Query("INSERT INTO swe3_orm.person (id, name) VALUES (10, 'seppi')");

            var listOfStudents = new List<Student>();
            var st1 = new Student();
            st1.Id = 1;
            st1.Name = "Stefan";
            

            // Generate new object
            var myTeacher = new Teacher();
            myTeacher.Id = 1;
            myTeacher.FirstName = "Fred";
            myTeacher.Name = "Keks";
            myTeacher.Salary = 1000;
            myTeacher.Gender = Gender.Female;

            var myClass = new Class
            {
                Id = 2,
                Name = "myClass",
                Teacher = myTeacher
            };
            
            // Save Object
            myClass.Save();
            //myClass.Delete();
            //myTeacher.Save();


            //var model = new Model(typeof(Student));

            Console.WriteLine();
            
        }
    }
}
