using System;
using OR_Mapper.Framework.Database;

namespace OR_Mapper.Framework
{
    /// <summary>
    /// This class can be used for creating entities that can be saved and deleted by interacting with a configured database class called Db.
    /// </summary>
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