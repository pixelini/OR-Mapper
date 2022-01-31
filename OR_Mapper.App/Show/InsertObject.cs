using System;
using OR_Mapper.App.ObjectClasses;

namespace OR_Mapper.App.Show
{
    public static class InsertObject
    {
        public static void Show()
        {
            Console.WriteLine();
            Console.WriteLine("1] Insert object");
            Console.WriteLine("---------------------------");

            var teacher = new Teacher
            {
                Id = 1,
                FirstName = "Hans",
                Name = "Hansson",
                Gender = Gender.Male,
                BirthDate = new DateTime(2000, 1, 1),
                HireDate = new DateTime(2020, 12, 31),
                Salary = 50000
            };

            teacher.Save();
        }
    }
}