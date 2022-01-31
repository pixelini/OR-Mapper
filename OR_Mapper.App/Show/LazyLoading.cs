using System;
using System.Linq;
using OR_Mapper.App.ObjectClasses;
using OR_Mapper.Framework.Database;

namespace OR_Mapper.App.Show
{
    public class LazyLoading
    {
        public static void Show()
        {
            Console.WriteLine();
            Console.WriteLine("6] Lazy Loading");
            Console.WriteLine("---------------------------");

            var listOfPersons = Db.GetAll<Teacher>();
            var teacher = listOfPersons.First();
            Console.WriteLine($"Class Count: {teacher.Classes}");

            Console.WriteLine("Access value");
            var x = teacher.Classes.Value;
            
            Console.WriteLine($"Class Count: {x.Count}");

            Console.WriteLine();
        }
    }
}