using System;
using System.ComponentModel.DataAnnotations;
using OR_Mapper.Framework;

namespace OR_Mapper.App.ObjectClasses
{
    public class Person : Entity
    {
        public string TableName = "Person";
        
        [Key]
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string FirstName { get; set; }
        
        public DateTime BirthDate { get; set; }    
        
        public Gender Gender { get; set; }
    }

    
    public enum Gender : int
    {
        Female = 0,
        Male = 1
    }
}