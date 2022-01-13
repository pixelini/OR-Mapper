using System;
using System.Collections.Generic;
using System.Linq;
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
            Db.ConnectionString = "Host=localhost;Username=postgres;Password=postgres;Database=postgres";

            //Db.Query("INSERT INTO swe3_orm.person (id, name) VALUES (10, 'seppi')");

            var listOfStudents = new List<Student>();
            var st1 = new Student();
            st1.Id = 1;
            st1.Name = "Stefan";

            // Generate new object
            var myTeacher = new Teacher();
            myTeacher.Id = 101;
            myTeacher.Name = "Max";
            myTeacher.FirstName = "Maier";
            myTeacher.Salary = 1000;
            myTeacher.Gender = Gender.Male;
            myTeacher.BirthDate = new DateTime(2000, 1, 1);
            myTeacher.HireDate = new DateTime(2022, 1, 1);
            
            var myClass = new Class
            {
                Id = 2,
                Name = "mySecondClass",
                Teacher = new Lazy<Teacher>(myTeacher)
            };

            var course = new Course
            {
                Id = 1,
                Name = "Lisi-Course",
                Teacher = new Lazy<Teacher>(),
                IsActive = true
            };
            
            myTeacher.Save();
            course.Save();
            
            // Save Object
            //myTeacher.Save();
            //myClass.Save();
            //myClass.Delete();

            //var listOfClasses = Db.GetAll<Class>();
            var listOfPersons = Db.GetAll<Course>();
            //var classWithId = Db.GetById<Class>(1);
            var x = listOfPersons.First().Teacher.Value;
            //var model = new Model(typeof(Student));
            Console.WriteLine();

        }
    }
}
