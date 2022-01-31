using System;
using System.Collections.Generic;
using OR_Mapper.App.ObjectClasses;
using OR_Mapper.Framework.Database;

namespace OR_Mapper.App.Show
{
    public class WithFk
    {
        public static void Show()
        {
            Console.WriteLine();
            Console.WriteLine("4] Create and load an object with foreign key");
            Console.WriteLine("---------------------------");

            var myTeacher = Db.GetById<Teacher>(1);
            
            var myClass = new Class
            {
                Id = 1,
                Name = "HisClass",
                Teacher = new Lazy<Teacher>(myTeacher)
            };

            /*var myCourse = new Course
            {
                Id = 1,
                Name = "ArtCourse",
                Teacher = new Lazy<Teacher>(myTeacher),
                IsActive = true
            };*/
            
            myClass.Save();
            //myCourse.Save();
            myTeacher.Save();

            Console.WriteLine("Load again from Db...");
            myClass = Db.GetById<Class>(1);
            Console.WriteLine($"Teacher {myClass.Teacher.Value.Name} teaches {myClass.Name}");

        }
    }
}