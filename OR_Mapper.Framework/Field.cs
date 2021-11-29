using System;
using System.Reflection;

namespace OR_Mapper.Framework
{
    public class Field
    {
        // Class information
        public Model Model { get; set; }
        public MemberInfo Member { get; set; }

        public Type Type
        {
            get
            {
                if (Member is PropertyInfo)
                {
                    return ((PropertyInfo)Member).PropertyType;
                }

                throw new NotSupportedException();
            }
        }
        
        
        // Database information
        public string ColumnName { get; set; }
        public Type ColumnType { get; set; }
        public bool IsPrimaryKey { get; set; }
        public bool IsForeignkey { get; set; }


        public Field(Model model)
        {
            Model = model;
        }
        
    }
}