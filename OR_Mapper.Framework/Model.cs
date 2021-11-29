using System;
using System.Collections.Generic;
using System.Linq;
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
            TableName = Member.Name;

            GetFields();
        }

        private void GetFields()
        {
            var properties = Member.GetProperties().Where(x => 
                x.PropertyType.IsValueType || 
                x.PropertyType == typeof(string) || 
                x.PropertyType == typeof(DateTime));
            
            foreach (var property in properties)
            {
                var field = new Field(property);
                Fields.Add(field);
            }
        }
    }
}