using System;

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
            // Saves data
            //Db.Insert()

        }
    }
}