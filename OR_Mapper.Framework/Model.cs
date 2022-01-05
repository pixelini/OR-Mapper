using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using OR_Mapper.Framework.Exceptions;
using OR_Mapper.Framework.Extensions;

namespace OR_Mapper.Framework
{
    public class Model
    {
        public Type Member { get; set; }
        public virtual string TableName { get; set; }
        public List<Field> Fields { get; set; } = new List<Field>();
        
        public List<ExternalField> ExternalFields { get; set; } = new List<ExternalField>();
        
        public List<ForeignKey> ForeignKeys { get; set; } = new List<ForeignKey>();
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
            _models = models;
            Member = memberType;
            TableName = GetTableName();
            GetFields();
        }

        private void GetFields()
        {
            // Check if discriminator is needed
            if (HasParentEntity(Member))
            {
                var discriminator = new Field("Discriminator", typeof(string), isDiscriminator: true);
                Fields.Add(discriminator);
            }

            // Find all base types
            var baseTypes = new List<Type>();
            var baseType = Member;

            while (baseType?.BaseType != typeof(Entity))
            {
                baseType = baseType?.BaseType;
                if (baseType is not null)
                {
                    baseTypes.Add(baseType);
                }
            }
            
            // Populate internal fields
            AddInternalFieldsFromType(Member);
            baseTypes.ForEach(AddInternalFieldsFromType);

            // Set primary key
            try
            {
                PrimaryKey = Fields.Single(x => x.IsPrimaryKey);
            }
            catch (InvalidOperationException)
            {
                throw new InvalidEntityException($"Please define a primary key on type {Member.Name} using the attribute {nameof(KeyAttribute)}");
            }
            
            // Populate external fields
            AddExternalFieldsFromType(Member);
            baseTypes.ForEach(AddExternalFieldsFromType);

        }

        private void AddInternalFieldsFromType(Type type)
        {
            var internalProperties = type.GetInternalProperties();
            var model = HasParentEntity(type) ? this : null;

            foreach (var property in internalProperties)
            {
                var field = new Field(property, model);
                Fields.Add(field);
            }
        }
        
        private void AddExternalFieldsFromType(Type type)
        {
            var externalProperties = type.GetExternalProperties();
            foreach (var externalProperty in externalProperties)
            {
                var externalField = GetExternalField(externalProperty);
                var externalModel = externalField.Model;
                var orderOfTheUniverse = Member.GUID.GetHashCode() > externalModel.Member.GUID.GetHashCode();

                // Add foreign keys
                if (orderOfTheUniverse && externalField.Relation is Relation.ManyToOne or Relation.OneToOne)
                {
                    var foreignPk = externalModel.PrimaryKey;
                    var fkName = $"fk_{externalModel.TableName}_{foreignPk.ColumnName}";
                    var fkField = new Field(fkName, foreignPk.ColumnType, isForeignKey: true);
                    var fk = new ForeignKey(fkField, externalModel.PrimaryKey, externalModel);
                    
                    Fields.Add(fkField);
                    ForeignKeys.Add(fk);
                }

                ExternalFields.Add(externalField);
            }
        }

        private ExternalField GetExternalField(PropertyInfo externalProperty)
        {
            var type = externalProperty.PropertyType;
            var propertyType = type;
            
            // if property type is collection, find element type of collection
            var isManyTo = type.IsList();

            if (isManyTo)
            {
                propertyType = type.GetGenericArguments().First();
            }

            // find corresponding property to determine if one-to-many/many-to-one/...
            var correspondingProperty = propertyType.GetCorrespondingPropertyOfType(Member);
            
            if (correspondingProperty is null)
            {
                throw new InvalidEntityException($"Please define corresponding property of type {Member.Name} on type {propertyType.Name}");
            }

            var correspondingPropertyType = correspondingProperty.PropertyType;
            var isToManyAtCorrespondingType = correspondingPropertyType.IsList();

            if (isManyTo && isToManyAtCorrespondingType)
            {
                return new ExternalField(GetModel(propertyType), Relation.ManyToMany);
            } 
            
            if (isManyTo && !isToManyAtCorrespondingType)
            {
                return new ExternalField(GetModel(propertyType), Relation.OneToMany);
            } 
            
            if (!isManyTo && isToManyAtCorrespondingType)
            {
                return new ExternalField(GetModel(propertyType), Relation.ManyToOne);
            }
            
            return new ExternalField(GetModel(propertyType), Relation.OneToOne);
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
                _models.Remove(this);
            }
     
            return model;
        }
    }

    public class ForeignKey
    {
        public Field LocalColumn { get; set; }
        
        public Field ForeignColumn { get; set; }

        public Model ForeignTable { get; set; }

        public ForeignKey(Field localColumn, Field foreignColumn, Model foreignTable)
        {
            LocalColumn = localColumn;
            ForeignColumn = foreignColumn;
            ForeignTable = foreignTable;
        }
    }
}