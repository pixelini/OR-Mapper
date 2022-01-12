using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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

        private void LoadRow<TEntity>(object record) where TEntity : new()
        {
            var type = record.GetType();
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
                        Db.LoadOneToOne(record, field);
                        break;
                }
            }
        }

    }
}