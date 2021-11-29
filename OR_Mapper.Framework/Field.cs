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
        
        
        public Field(PropertyInfo propertyInfo, Model? model)
        {
            var prefix = model is null ? string.Empty : $"{model.Member.Name}_";
            Member = propertyInfo;
            ColumnName = prefix + propertyInfo.Name;
            ColumnType = propertyInfo.PropertyType;
            
            var keyAttributes = propertyInfo.GetCustomAttributes(typeof(KeyAttribute)).ToList();
            IsPrimaryKey = keyAttributes.Any();

        }
        
    }
}