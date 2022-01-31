using System;
using OR_Mapper.App.ObjectClasses;
using OR_Mapper.Framework.Database;

namespace OR_Mapper.App.Show
{
    public static class EditObject
    {
        public static void Show()
        {
            Console.WriteLine();
            Console.WriteLine("2] Load and edit object");
            Console.WriteLine("---------------------------");

            var teacher = Db.GetById<Teacher>(1);

            Console.WriteLine($"Loaded element: Id: {teacher.Id}, Firstname: {teacher.FirstName}, Name: {teacher.Name}, Salary: {teacher.Salary}");
            Console.WriteLine("Change name and increase salary to 100000...");
            
            teacher.FirstName = "NotHansAnymore";
            teacher.Salary = 100000;

            Console.WriteLine("Save in Db...");
            teacher.Save();

            Console.WriteLine("Load again from Db...");
            teacher = Db.GetById<Teacher>(1);
            Console.WriteLine($"Loaded element: Id: {teacher.Id}, Firstname: {teacher.FirstName}, Name: {teacher.Name}, Salary: {teacher.Salary}");
        }
    }
}