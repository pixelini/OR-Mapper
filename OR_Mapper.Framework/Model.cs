using System;
using System.Collections.Generic;
using OR_Mapper.Framework.Database;

namespace OR_Mapper.Framework
{
    public class Model
    {
        public Type Member { get; set; }
        public virtual string TableName { get; set; }
        public List<Field> Fields { get; set; }
        public Field PrimaryKey { get; set; }

        public void Save()
        {
            // Saves data
            //Db.Insert()

        }
    }
}