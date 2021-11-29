using OR_Mapper.Framework;

namespace OR_Mapper.App.ObjectClasses
{
    public class Course : Entity
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsActive { get; set; }

        public Teacher Teacher { get; set; }
    }
}