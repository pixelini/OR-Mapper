using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Npgsql.Replication.TestDecoding;
using OR_Mapper.Framework.Exceptions;

namespace OR_Mapper.Framework.Database
{
    /// <summary>
    /// This class is used to load objects from different types.
    /// </summary>
    public class ObjectLoader
    {
        /// <summary>
        /// Holds the data reader.
        /// </summary>
        public IDataReader Reader { get; set; }
        
        /// <summary>
        /// Constructs a new object loader instance.
        /// </summary>
        /// <param name="reader">Data reader.</param>
        public ObjectLoader(IDataReader reader)
        {
            Reader = reader;
        }
        
        /// <summary>
        /// Loads a collection from reader.
        /// </summary>
        /// <typeparam name="TEntity">Class of entity.</typeparam>
        /// <returns>Entity result list.</returns>
        public List<TEntity> LoadCollection<TEntity>() where TEntity : new()
        {
            var list = new List<TEntity>();

            while (Reader.Read())
            {
                var record = new TEntity();
                LoadRow<TEntity>(record);
                list.Add(record);
            }
            
            return list;
        }
        
        /// <summary>
        /// Loads a single entity from reader.
        /// </summary>
        /// <typeparam name="TEntity">Class of entity.</typeparam>
        /// <returns>Result entity.</returns>
        public TEntity LoadSingle<TEntity>() where TEntity : new()
        {
            var record = new TEntity();

            if (!Reader.Read())
            {
                return record;
            }

            LoadRow<TEntity>(record);
            return record;
        }

        /// <summary>
        /// Loads a row from a record.
        /// </summary>
        /// <param name="record">Record.</param>
        /// <typeparam name="TEntity">Class of entity.</typeparam>
        /// <exception cref="InvalidEntityException">InvalidEntityException.</exception>
        private void LoadRow<TEntity>(TEntity record) where TEntity : new()
        {
            var type = typeof(TEntity);
            var model = new Model(type);
            
            foreach (var field in model.Fields)
            {
                // Assign properties
                var value = Reader[field.ColumnName];
                value = value == DBNull.Value ? default : value;
                field.SetValue(record, value);
            }

            var currentType = GetType();
            
            foreach (var field in model.ExternalFields)
            {
                const BindingFlags methodFlags = BindingFlags.Instance | BindingFlags.NonPublic;
                
                var loadMethodType = field.Relation switch
                {
                    Relation.OneToMany => currentType?
                        .GetMethod(nameof(ConstructLoadOneToMany), methodFlags)?
                        .MakeGenericMethod(field.Model.Member),
                    Relation.ManyToOne => currentType?
                        .GetMethod(nameof(ConstructLoadOneToOne), methodFlags)?
                        .MakeGenericMethod(field.Model.Member),
                    Relation.ManyToMany =>
                        // TO DO: Implement many to many
                        currentType?
                            .GetMethod(nameof(Db.LoadManyToMany), methodFlags)?
                            .MakeGenericMethod(field.Model.Member),
                    Relation.OneToOne => currentType?
                        .GetMethod(nameof(ConstructLoadOneToOne), methodFlags)?
                        .MakeGenericMethod(field.Model.Member),
                    _ => throw new InvalidEntityException("")
                };

                var result = loadMethodType?.Invoke(this, new object?[] { record, field });
                field.SetValue(record, result);
            }
        }

        /// <summary>
        /// Helper method for constructing OneToOne loading.
        /// </summary>
        /// <param name="record">Record.</param>
        /// <param name="field">Field.</param>
        /// <typeparam name="TCorrespondingType">Type of corresponding class.</typeparam>
        private Func<TCorrespondingType> ConstructLoadOneToOne<TCorrespondingType>(object record, ExternalField field) 
            where TCorrespondingType : new()
        {
            return () => Db.LoadOneToOne<TCorrespondingType>(record, field);
        }
        
        /// <summary>
        /// Helper method for constructing OneToMany loading.
        /// </summary>
        /// <param name="record">Record.</param>
        /// <param name="field">Field.</param>
        /// <typeparam name="TCorrespondingType">Type of corresponding class.</typeparam>
        private Func<List<TCorrespondingType>> ConstructLoadOneToMany<TCorrespondingType>(object record, ExternalField field) 
            where TCorrespondingType : new()
        {
            return () => Db.LoadOneToMany<TCorrespondingType>(record, field);
        }
    }
}