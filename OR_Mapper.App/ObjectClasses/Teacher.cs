using System;
using System.Collections.Generic;

namespace OR_Mapper.App.ObjectClasses
{
    public class Teacher : Person
    {
        public int Salary { get; set; }
        
        public DateTime HireDate { get; set; }

        public List<Class> Classes { get; private set; } = new List<Class>();
    }
}