namespace OR_Mapper.App.ObjectClasses
{
    public class Student : Person

    {
        public string Grade { get; set; }

        public Class Class { get; set; }
    }
}