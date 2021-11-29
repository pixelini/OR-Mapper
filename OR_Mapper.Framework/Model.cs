using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OR_Mapper.Framework.Database;

namespace OR_Mapper.Framework
{
    public class Model
    {
        public Type Member { get; set; }
        public virtual string TableName { get; set; }
        public List<Field> Fields { get; set; } = new List<Field>();
        public Field PrimaryKey { get; set; }

        public Model(Type memberType)
        {
            Member = memberType;
            TableName = GetTableName();
            GetFields();
        }

        private void GetFields()
        {
            GetFieldsFromType(Member);
            
            try
            {
                PrimaryKey = Fields.Single(x => x.IsPrimaryKey);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
                //TODO: throw new InvalidEntityException();
            }
            
            var baseType = Member;

            while (baseType?.BaseType != typeof(Entity))
            {
                baseType = baseType?.BaseType;
                if (baseType is not null)
                {
                    GetFieldsFromType(baseType);
                }
            }

        }

        private void GetFieldsFromType(Type type)
        {
            var properties = type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).Where(x => 
                x.PropertyType.IsValueType || 
                x.PropertyType == typeof(string) || 
                x.PropertyType == typeof(DateTime));
            
            var model = HasParentEntity(type) ? this : null;

            foreach (var property in properties)
            {
                var field = new Field(property, model);
                Fields.Add(field);
            }
        }

        private bool HasParentEntity(Type type)
        {
            return type.BaseType != typeof(Entity);
        }

        private string GetTableName()
        {
            var baseType = Member;

            while (baseType?.BaseType != typeof(Entity))
            {
                baseType = baseType?.BaseType;
            }
            return baseType.Name;
        }
        
        
    }
}