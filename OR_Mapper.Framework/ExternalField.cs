using System.Linq;
using System.Linq.Expressions;
using OR_Mapper.Framework.Extensions;

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
        
        public object? GetValue(object obj)
        {
            var type = obj.GetType();

            var correspondingProperty = type
                .GetProperties()
                .First(x => 
                    Model.Member == (x.PropertyType.IsList() ? x.PropertyType.GetGenericArguments().First() : x.PropertyType));

            return correspondingProperty.GetValue(obj);
        }
    }
}