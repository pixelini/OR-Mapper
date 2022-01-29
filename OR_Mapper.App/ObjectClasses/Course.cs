using System;
using System.ComponentModel.DataAnnotations;
using OR_Mapper.Framework;

namespace OR_Mapper.App.ObjectClasses
{
    public class Course : Entity
    {
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public bool IsActive { get; set; }

        public Lazy<Teacher> Teacher { get; set; }
    }
}