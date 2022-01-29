using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Npgsql.Replication.TestDecoding;
using OR_Mapper.Framework.Exceptions;

namespace OR_Mapper.Framework.Database
{
    public class ObjectLoader
    {
        public IDataReader Reader { get; set; }
        
        
        public ObjectLoader(IDataReader reader)
        {
            Reader = reader;
        }
        
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
                        // TODO: Implement many to many
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

        private Func<TCorrespondingType> ConstructLoadOneToOne<TCorrespondingType>(object record, ExternalField field) 
            where TCorrespondingType : new()
        {
            return () => Db.LoadOneToOne<TCorrespondingType>(record, field);
        }
        
        private Func<List<TCorrespondingType>> ConstructLoadOneToMany<TCorrespondingType>(object record, ExternalField field) 
            where TCorrespondingType : new()
        {
            return () => Db.LoadOneToMany<TCorrespondingType>(record, field);
        }
    }
}