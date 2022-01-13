using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using OR_Mapper.Framework.Extensions;

namespace OR_Mapper.Framework
{
    public class Field
    {
        public PropertyInfo PropertyInfo { get; set; }
        
        // Database information
        public string ColumnName { get; set; }
        public Type ColumnType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignKey { get; set; }
        public bool IsDiscriminator { get; set; }

        public Field(PropertyInfo propertyInfo, Model? model)
        {
            var prefix = model is null ? string.Empty : $"{model.Member.Name}_";
            PropertyInfo = propertyInfo;
            ColumnName = prefix + propertyInfo.Name;
            ColumnType = propertyInfo.PropertyType;
            
            var keyAttributes = propertyInfo.GetCustomAttributes(typeof(KeyAttribute)).ToList();
            IsPrimaryKey = keyAttributes.Any();
        }

        public Field(string name, Type type, bool isPrimaryKey = false, bool isDiscriminator = false, bool isForeignKey = false)
        {
            ColumnName = name;
            ColumnType = type;
            IsPrimaryKey = isPrimaryKey;
            IsForeignKey = isForeignKey;
            IsDiscriminator = isDiscriminator;
        }

        public object? GetValue(object obj)
        {
            if (IsForeignKey)
            {
                return default;
            }
            
            var type = obj.GetType();
            
            if (IsDiscriminator)
            {
                return type.Name;
            }

            return type
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(x => x.Name == PropertyInfo.Name)?
                .GetValue(obj);
        }

        public void SetValue(object obj, object value)
        {
            if (IsForeignKey || IsDiscriminator)
            {
                return;
            }
            
            var type = obj.GetType();
            type.GetProperties().FirstOrDefault(x => x.Name == PropertyInfo.Name)?.SetValue(obj, value);
        }
    }
}