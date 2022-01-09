using System;
using OR_Mapper.Framework.Database;

namespace OR_Mapper.Framework
{
    public class Entity
    {
        public Entity()
        {
            var type = GetType();
            Console.WriteLine();
        }
        
        public void Save()
        {
            Db.Save(this);
        }
        
        public void Delete()
        {
            Db.Delete(this);
        }
    }
}