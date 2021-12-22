using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using OR_Mapper.Framework.Extensions;

namespace OR_Mapper.Framework
{
    public class Model
    {
        public Type Member { get; set; }
        public virtual string TableName { get; set; }
        public List<Field> Fields { get; set; } = new List<Field>();
        
        public List<ExternalField> ExternalFields { get; set; } = new List<ExternalField>();
        public Field PrimaryKey { get; set; }

        private readonly List<Model> _models = new List<Model>();

        public Model(Type memberType)
        {
            Member = memberType;
            TableName = GetTableName();
            GetFields();
        }

        private Model(Type memberType, List<Model> models)
        {
            Member = memberType;
            _models = models;
            TableName = GetTableName();
            GetFields();
        }

        private void GetFields()
        {
            GetFieldsFromType(Member);

            var baseType = Member;

            while (baseType?.BaseType != typeof(Entity))
            {
                baseType = baseType?.BaseType;
                if (baseType is not null)
                {
                    GetFieldsFromType(baseType);
                }
            }
            
            try
            {
                PrimaryKey = Fields.Single(x => x.IsPrimaryKey);
            }
            catch (InvalidOperationException e)
            {
                Console.WriteLine(e);
                //TODO: throw new InvalidEntityException();
            }

        }
        
        private void GetFieldsFromType(Type type)
        {
            var internalProperties = type.GetInternalProperties();
            var model = HasParentEntity(type) ? this : null;

            foreach (var property in internalProperties)
            {
                var field = new Field(property, model);
                Fields.Add(field);
            }

            var externalProperties = type.GetExternalProperties();
            foreach (var externalProperty in externalProperties)
            {
                var externalField = GetExternalField(externalProperty);
                ExternalFields.Add(externalField);
            }
        }

        private ExternalField GetExternalField(PropertyInfo externalProperty)
        {
            var type = externalProperty.PropertyType;
            var propertyType = type;
            var isManyTo = type.IsList();

            if (isManyTo)
            {
                propertyType = type.GetGenericArguments().First();
            }

            var correspondingProperty = propertyType.GetCorrespondingPropertyOfType(Member);
            if (correspondingProperty is null)
            {
                //TODO: throw new InvalidEntityException();
            }

            var correspondingPropertyType = correspondingProperty.PropertyType;
            var isToManyAtCorrespondingType = correspondingPropertyType.IsList();

            if (isToManyAtCorrespondingType)
            {
                correspondingPropertyType = correspondingPropertyType.GetGenericArguments().First();
            }

            if (isManyTo && isToManyAtCorrespondingType)
            {
                return new ExternalField(GetModel(correspondingPropertyType), Relation.ManyToMany);
            } 
            else if (isManyTo && !isToManyAtCorrespondingType)
            {
                return new ExternalField(GetModel(correspondingPropertyType), Relation.OneToMany);
            } 
            else if (!isManyTo && isToManyAtCorrespondingType)
            {
                return new ExternalField(GetModel(correspondingPropertyType), Relation.ManyToOne);
            }
            else
            {
                return new ExternalField(GetModel(correspondingPropertyType), Relation.OneToOne);
            }

        }

        private bool HasParentEntity(Type type)
        {
            return type.BaseType != typeof(Entity);
        }
        
        private string GetTableName()
        {
            var baseType = Member;

            while (baseType?.BaseType != typeof(Entity))
            {
                baseType = baseType?.BaseType;
            }
            return baseType.Name;
        }

        // if current member is already in models list (table already exists), it is returned and deleted from list
        // if not, a new model
        private Model GetModel(Type type)
        {
            var model = _models.FirstOrDefault(x => x.Member == type);
            if (model is null)
            {
                _models.Add(this);
                model = new Model(type, _models);
                _models.Remove(model);
            }
     
            return model;

        }
        
        
    }
}