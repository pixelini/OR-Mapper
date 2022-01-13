using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using Npgsql.Replication.TestDecoding;

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

            foreach (var field in model.ExternalFields)
            {
                switch (field.Relation)
                {
                    case Relation.OneToMany:
                        Db.LoadOneToMany(record, field);
                        break;
                    case Relation.ManyToOne:
                        Db.LoadManyToOne(record, field);
                        break;
                    case Relation.ManyToMany:
                        Db.LoadManyToMany(record, field);
                        break;
                    case Relation.OneToOne:
                        var constructLoadMethod = GetType()
                            .GetMethod(nameof(ConstructLoadOneToOne), BindingFlags.Instance | BindingFlags.NonPublic)
                            .MakeGenericMethod(field.Model.Member);
                        
                        var loadMethod = constructLoadMethod.Invoke(this, new object?[] { record, field });
                        field.SetValue(record, loadMethod);
                        break;

                }
            }
        }

        private Func<TCorrespondingType> ConstructLoadOneToOne<TCorrespondingType>(object record, ExternalField field) 
            where TCorrespondingType : new()
        {
            return () => Db.LoadOneToOne<TCorrespondingType>(record, field);
        }
    }
}