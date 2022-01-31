using System;
using System.Collections.Generic;

namespace OR_Mapper.App.ObjectClasses
{
    public class Teacher : Person
    {
        public int Salary { get; set; }
        
        public DateTime HireDate { get; set; }

        public Lazy<List<Class>> Classes { get; set; } = new Lazy<List<Class>>();

        public Lazy<Course> Course { get; set; }
    }
}