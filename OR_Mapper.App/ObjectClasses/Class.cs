using OR_Mapper.Framework;

namespace OR_Mapper.App.ObjectClasses
{
    public class Class : Entity
    {
        public string Id { get; set; }
        public string Name { get; set; } 
        
        public Teacher Teacher { get; set; }

    }
}