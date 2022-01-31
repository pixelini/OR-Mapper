using System;
using OR_Mapper.App.ObjectClasses;
using OR_Mapper.Framework.Database;

namespace OR_Mapper.App.Show
{
    public class WithFkList
    {
        public static void Show()
        {
            Console.WriteLine();
            Console.WriteLine("5] Load teacher and show classes");
            Console.WriteLine("---------------------------");

            var myTeacher = Db.GetById<Teacher>(1);
            
            var anotherClass = new Class
            {
                Id = 2,
                Name = "HisSecondClass",
                Teacher = new Lazy<Teacher>(myTeacher)
            };
            
            anotherClass.Save();

            Console.WriteLine(myTeacher.FirstName + " " + myTeacher.Name + " teaches: ");
            
            foreach(var c in myTeacher.Classes.Value)
            {
                Console.WriteLine(c.Name);
            }
            
            Console.WriteLine();

        }
    }
}