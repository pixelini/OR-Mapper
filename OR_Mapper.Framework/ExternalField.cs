namespace OR_Mapper.Framework
{
    public class ExternalField
    {
        public Model Model { get; set; }

        public Relation Relation { get; set; }


        public ExternalField(Model model, Relation relation)
        {
            Model = model;
            Relation = relation;
        }
    }
}