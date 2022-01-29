using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using OR_Mapper.Framework.Exceptions;
using OR_Mapper.Framework.Extensions;

namespace OR_Mapper.Framework
{
    /// <summary>
    /// This class holds all model/entity metadata.
    /// </summary>
    public class Model
    {
        /// <summary>
        /// Holds the Member type.
        /// </summary>
        public Type Member { get; set; }
        
        /// <summary>
        /// Holds the table name.
        /// </summary>
        public virtual string TableName { get; set; }
        
        /// <summary>
        /// Holds all internal fields.
        /// </summary>
        public List<Field> Fields { get; set; } = new List<Field>();
        
        /// <summary>
        /// Holds all external fields.
        /// </summary>
        public List<ExternalField> ExternalFields { get; set; } = new List<ExternalField>();
        
        /// <summary>
        /// Holds all foreign keys.
        /// </summary>
        public List<ForeignKey> ForeignKeys { get; set; } = new List<ForeignKey>();
        
        /// <summary>
        /// Holds the primary key field.
        /// </summary>
        public Field PrimaryKey { get; set; }
        
        /// <summary>
        /// Holds models.
        /// </summary>
        private readonly List<Model> _models = new List<Model>();

        
        /// <summary>
        /// Creates a new instance of model class.
        /// </summary>
        /// <param name="memberType">Member type.</param>
        public Model(Type memberType)
        {
            Member = memberType;
            TableName = GetTableName();
            GetFields();
        }

        /// <summary>
        /// Creates a new instance of model class.
        /// </summary>
        /// <param name="memberType">Member type.</param>
        /// <param name="models">List of models.</param>
        private Model(Type memberType, List<Model> models)
        {
            _models = models;
            Member = memberType;
            TableName = GetTableName();
            GetFields();
        }

        /// <summary>
        /// Gets the model fields.
        /// </summary>
        /// <exception cref="InvalidEntityException">InvalidEntityException.</exception>
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

        /// <summary>
        /// Fills the internal fields from given type in the fields property.
        /// </summary>
        /// <param name="type">Type.</param>
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
        
        /// <summary>
        /// Fills the external fields from given type in the external fields property.
        /// </summary>
        /// <param name="type">Type.</param>
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
                    var isChildClass = externalModel.HasParentEntity(externalField.GetType());
                    var fkName = isChildClass ? $"{externalProperty.Name}_id_fk" : $"{externalModel.TableName}_id_fk";
                    var fkField = new Field(fkName, foreignPk.ColumnType, isForeignKey: true);
                    var fk = new ForeignKey(fkField, externalModel.PrimaryKey, externalModel);
                    
                    Fields.Add(fkField);
                    ForeignKeys.Add(fk);
                }

                ExternalFields.Add(externalField);
            }
        }

        /// <summary>
        /// Creates an external field from given property info of external field.
        /// </summary>
        /// <param name="externalProperty">External property.</param>
        /// <returns>External field.</returns>
        /// <exception cref="InvalidEntityException">InvalidEntityException.</exception>
        private ExternalField GetExternalField(PropertyInfo externalProperty)
        {
            // if lazy, find element type of lazy type
            var type = externalProperty.PropertyType.GetUnderlyingTypeForLazy();
            
            // if type is a collection, find element type of collection
            var isManyTo = type.IsList();
            type = type.GetUnderlyingType();

            // find corresponding property to determine if one-to-many/many-to-one/...
            var correspondingProperty = type.GetCorrespondingPropertyOfType(Member);
            
            if (correspondingProperty is null)
            {
                throw new InvalidEntityException($"Please define corresponding property of type {Member.Name} on type {type.Name}");
            }

            var correspondingPropertyType = correspondingProperty.PropertyType.GetUnderlyingTypeForLazy();
            var isToManyAtCorrespondingType = correspondingPropertyType.IsList();

            if (isManyTo && isToManyAtCorrespondingType)
            {
                return new ExternalField(GetModel(type), Relation.ManyToMany);
            } 
            
            if (isManyTo && !isToManyAtCorrespondingType)
            {
                return new ExternalField(GetModel(type), Relation.OneToMany);
            } 
            
            if (!isManyTo && isToManyAtCorrespondingType)
            {
                return new ExternalField(GetModel(type), Relation.ManyToOne);
            }
            
            return new ExternalField(GetModel(type), Relation.OneToOne);
        }

        /// <summary>
        /// Checks if give type has a parent entity.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns>True if type is an inherited type.</returns>
        private bool HasParentEntity(Type type)
        {
            return type.BaseType != typeof(Entity);
        }
        
        /// <summary>
        /// Gets the table name for entity or inherited entity.
        /// </summary>
        /// <returns>Table name.</returns>
        private string GetTableName()
        {
            var baseType = Member;

            while (baseType?.BaseType != typeof(Entity))
            {
                baseType = baseType?.BaseType;
            }
            return baseType.Name;
        }
        
        /// <summary>
        /// Gets model from given type. If current member is already in models list (table already exists),
        /// it is returned and deleted from list. If not, a new model is created.
        /// </summary>
        /// <param name="type"></param>
        /// <returns>Model.</returns>
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
}