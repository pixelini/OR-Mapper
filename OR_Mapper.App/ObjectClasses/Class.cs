using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OR_Mapper.Framework;

namespace OR_Mapper.App.ObjectClasses
{
    public class Class : Entity
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; } 
        
        public Lazy<Teacher> Teacher { get; set; }

        public Lazy<List<Student>> Student { get; set; }
    }
}