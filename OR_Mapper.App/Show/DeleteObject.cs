using System;
using OR_Mapper.App.ObjectClasses;
using OR_Mapper.Framework.Database;

namespace OR_Mapper.App.Show
{
    public class DeleteObject
    {
        public static void Show()
        {
            Console.WriteLine();
            Console.WriteLine("3] Delete object");
            Console.WriteLine("---------------------------");

            var teacher = new Teacher
            {
                Id = 2,
                FirstName = "TempFritz",
                Name = "TempPhantom",
                Gender = Gender.Male,
                BirthDate = new DateTime(2000, 1, 1),
                HireDate = new DateTime(2020, 12, 31),
                Salary = 300
            };

            teacher.Save();
            
            teacher = Db.GetById<Teacher>(2);
            Console.WriteLine($"Loaded element: Id: {teacher.Id}, Firstname: {teacher.FirstName}, Name: {teacher.Name}, Salary: {teacher.Salary}");
            
            teacher.Delete();
            teacher = Db.GetById<Teacher>(2);
            Console.WriteLine($"Loaded element: Id: {teacher.Id}, Firstname: {teacher.FirstName}, Name: {teacher.Name}, Salary: {teacher.Salary}");


        }
    }
}