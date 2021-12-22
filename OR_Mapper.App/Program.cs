using System;
using OR_Mapper.App.ObjectClasses;
using OR_Mapper.Framework;

namespace OR_Mapper.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            /*var myTeacher = new Teacher();
            myTeacher.FirstName = "Fred";
            myTeacher.Name = "Keks";
            myTeacher.Salary = 1000;*/
     
            var model = new Model(typeof(Teacher));

            Console.WriteLine();
            
        }
    }
}
