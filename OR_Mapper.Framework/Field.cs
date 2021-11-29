using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace OR_Mapper.Framework
{
    public class Field
    {
        public PropertyInfo Member { get; set; }

        public Type Type => Member.PropertyType;


        // Database information
        public string ColumnName { get; set; }
        public Type ColumnType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }


        public Field(PropertyInfo propertyInfo)
        {
            Member = propertyInfo;
            ColumnName = propertyInfo.Name;
            ColumnType = propertyInfo.PropertyType;
            
            var keyAttributes = propertyInfo.GetCustomAttributes(typeof(KeyAttribute)).ToList();
            IsPrimaryKey = keyAttributes.Any();
        }
        
    }
}