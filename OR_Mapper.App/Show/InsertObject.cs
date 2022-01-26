using System;
using OR_Mapper.App.ObjectClasses;

namespace OR_Mapper.App.Show
{
    public class InsertObject
    {
        public static void Show()
        {
            Console.WriteLine("1] Insert object");
            Console.WriteLine("---------------------------");

            var t = new Teacher
            {
                Id = 1,
                FirstName = "Jerry",
                Salary = 50000
            };

            t.Save();
        }
    }
}