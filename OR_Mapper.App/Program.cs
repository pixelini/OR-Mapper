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

            InsertObject.Show();
            EditObject.Show();


            /*
            
            
            
            
            
            
            
            

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
            var listOfPersons = Db.GetAll<Teacher>();

            var teacher = listOfPersons.First();
            var x = teacher.Classes.Value;
            var y = teacher.Course.Value;

            foreach (var z in x)
            {
                Console.WriteLine($"My teacher (class {z.Id}) is {z.Teacher.Value.Name}!");
            }
            
            FluentApi.UseConnection(() => new NpgsqlConnection(Db.ConnectionString));

            var mytest1 = FluentApi.Entity<Student>().Where("FirstName").Is("Maier").Execute();
            var mytest2 = FluentApi.Entity<Student>().Where("FirstName").Is("Maier").And("Grade").Is(null).Execute();
            var mytest3 = FluentApi.Entity<Teacher>().Max("Salary").Execute();
            var mytest4 = FluentApi.Entity<Teacher>().Min("Salary").Execute();
            var mytest5 = FluentApi.Entity<Teacher>().Avg("Salary").Execute();

            //var classWithId = Db.GetById<Class>(1);
            //var x = listOfPersons.First().Teacher.Value;
            //var model = new Model(typeof(Student));
            Console.WriteLine();
            
            
            
            
            
            
            */

        }
    }
}
