using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace OR_Mapper.Framework
{
    /// <summary>
    /// This class holds all field metadata.
    /// </summary>
    public class Field
    {
        /// <summary>
        /// Holds the property info.
        /// </summary>
        public PropertyInfo PropertyInfo { get; set; }
        
        /// <summary>
        /// Holds the column name.
        /// </summary>
        public string ColumnName { get; set; }
        
        /// <summary>
        /// Holds the column type.
        /// </summary>
        public Type ColumnType { get; set; }
        
        /// <summary>
        /// Describes if field is primary key.
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        
        /// <summary>
        /// Describes if field is foreign key.
        /// </summary>
        public bool IsForeignKey { get; set; }
        
        /// <summary>
        /// Describes if field is discriminator field.
        /// </summary>
        public bool IsDiscriminator { get; set; }
        
        
        /// <summary>
        /// Constructor for internal fields.
        /// </summary>
        /// <param name="propertyInfo">Property Info.</param>
        /// <param name="model">Model.</param>
        public Field(PropertyInfo propertyInfo, Model? model)
        {
            var prefix = model is null ? string.Empty : $"{model.Member.Name}_";
            PropertyInfo = propertyInfo;
            ColumnName = prefix + propertyInfo.Name;
            ColumnType = propertyInfo.PropertyType;
            
            var keyAttributes = propertyInfo.GetCustomAttributes(typeof(KeyAttribute)).ToList();
            IsPrimaryKey = keyAttributes.Any();
        }

        /// <summary>
        /// Constructor for external fields.
        /// </summary>
        /// <param name="name">Name.</param>
        /// <param name="type">Type.</param>
        /// <param name="isPrimaryKey">Boolean that defines whether the field is a primary key field.</param>
        /// <param name="isDiscriminator">Boolean that defines whether the field is a discriminator field.</param>
        /// <param name="isForeignKey">Boolean that defines whether the field is a foreign key field.</param>
        public Field(string name, Type type, bool isPrimaryKey = false, bool isDiscriminator = false, bool isForeignKey = false)
        {
            ColumnName = name;
            ColumnType = type;
            IsPrimaryKey = isPrimaryKey;
            IsForeignKey = isForeignKey;
            IsDiscriminator = isDiscriminator;
        }

        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <returns>Field value.</returns>
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

        /// <summary>
        /// Sets the field value.
        /// </summary>
        /// <param name="obj">Object.</param>
        /// <param name="value">Value.</param>
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