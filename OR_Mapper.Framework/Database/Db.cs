using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Npgsql;
using OR_Mapper.Framework.Extensions;

namespace OR_Mapper.Framework.Database
{
    public static class Db
    {
        private static string _connectionString;

        public static IDbConnection Connection { get; set; }
        
        public static object? Insert(string sql)
        {
            var result = Query(sql);
            return result ?? null;
        }

        public static IDbConnection GetInstance()
        {
            if (Connection is null)
            {
                return new NpgsqlConnection(_connectionString);
            }

            return Connection;
        }

        // sends query to database which executes it
        public static IDataReader Query(string sql)
        {
            using var cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Prepare();
            
            try
            {
                var result = cmd.ExecuteReader();
                return result;
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine("NpgsqlException Error Message ex.Message: " + ex.Message);
            }
            
            return null;
        }
        
        public static object Fetch(string sql)
        {
            var name = "";
            
            var result = Query(sql);

            if (result == null)
            {
                // throw exception
            }
            
            while (result.Read())
            {
                name = result.GetString(0);
            }

            return name;

        }

        public static void Save(Entity entity)
        {
            var conn = Connect();
            var type = entity.GetType();
            var model = new Model(type);

            string sql = $"INSERT INTO {model.TableName} (";
            var columnNames = model.Fields.Select(x => x.ColumnName);
            sql += string.Join(',', columnNames);
            sql += ") VALUES (";
            var cmd = conn.CreateCommand();

            var counter = 0;
            var modelFieldsWithoutFk = model.Fields.Where(x => !x.IsForeignKey).ToList();
            
            // Internal fields
            foreach (var fields in modelFieldsWithoutFk)
            {
                sql += "@p" + counter;
                
                if (counter < modelFieldsWithoutFk.Count-1)
                {
                    sql += ",";
                }
                
                cmd.AddParameter("@p" + counter, fields.GetValue(entity) ?? DBNull.Value);
                counter++;
            }

            var modelFieldsWithFk = model.Fields.Where(x => x.IsForeignKey).ToList();
            
            // One to one and many to one (fk)
            var upperBoundary = modelFieldsWithFk.Count + counter - 1;
            foreach (var fields in modelFieldsWithFk)
            {
                sql += "@p" + counter;
                
                if (counter < upperBoundary)
                {
                    sql += ",";
                }

                var foreignKey = model.ForeignKeys.First(x => x.LocalColumn == fields);
                var correspondingField = model.ExternalFields.First(x => x.Model == foreignKey.ForeignTable);
                
                var externalEntity = correspondingField.GetValue(entity);
                var pk = correspondingField.Model.PrimaryKey.GetValue(externalEntity);
                cmd.AddParameter("@p" + counter, pk ?? DBNull.Value);
                counter++;
            }

            sql += ")";
            cmd.CommandText = sql;
        }

        public static IDbConnection Connect()
        {
            try
            {
                var conn = GetInstance();
                conn.Open();
                return conn;
            }
            catch (NpgsqlException ex)
            {
                //TODO: throw DbException;
            }

            return null;
        }
        
    }
}