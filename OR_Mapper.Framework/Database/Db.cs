using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Npgsql;
using Npgsql.Replication.TestDecoding;
using OR_Mapper.Framework.Extensions;

namespace OR_Mapper.Framework.Database
{
    public static class Db
    {
        private static string _connectionString;

        public static string DbSchema { get; set; }

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

            string sql = DbSchema is not null ? $"INSERT INTO {DbSchema}.{model.TableName} (" : $"INSERT INTO {model.TableName} (";

            var columnNames = model.Fields.Select(x => x.ColumnName);
            sql += string.Join(',', columnNames);
            sql += ") VALUES (";
            var cmd = conn.CreateCommand();

            var modelFieldsWithoutFk = model.Fields.Where(x => !x.IsForeignKey).ToList();
            var insertParameters = new List<string>();
            var parameterCount = 0;
            
            // Build parameters for internal fields
            foreach (var fields in modelFieldsWithoutFk)
            {
                string paramName = "@p" + parameterCount;
                insertParameters.Add(paramName);
                
                var paramValue = fields.GetValue(entity) ?? DBNull.Value;
                
                if (fields.ColumnType.IsEnum && paramValue != DBNull.Value)
                {
                    paramValue = (int) paramValue;
                }
                
                cmd.AddParameter(paramName, paramValue);
                parameterCount++;
            }

            var modelFieldsWithFk = model.Fields.Where(x => x.IsForeignKey).ToList();
            
            // Build parameters for foreign keys
            foreach (var fields in modelFieldsWithFk)
            {
                string paramName = $"@p{parameterCount}";
                insertParameters.Add(paramName);

                var foreignKey = model.ForeignKeys.First(x => x.LocalColumn == fields);
                var correspondingField = model.ExternalFields.First(x => x.Model == foreignKey.ForeignTable);
		
                var externalEntity = correspondingField.GetValue(entity);
                var pk = correspondingField.Model.PrimaryKey.GetValue(externalEntity);
                cmd.AddParameter(paramName, pk ?? DBNull.Value);
                parameterCount++;
            }
            
            sql += string.Join(',', insertParameters);
            sql += ") ";
	
            // Update row on duplicate primary key
            sql += $"ON CONFLICT ({model.PrimaryKey.ColumnName}) DO UPDATE SET ";
	
            parameterCount = 0;
            var updateParameters = new List<string>();
            
            foreach (var column in columnNames)
            {
                string paramName = $"{column} = @p{parameterCount}";
                updateParameters.Add(paramName);
                parameterCount++;
            }
            
            sql += string.Join(',', updateParameters);

            // Return primary key
            sql += $" RETURNING {model.PrimaryKey.ColumnName}";

            // Execute command
            try
            {
                cmd.CommandText = sql;
                var success = cmd.ExecuteScalar();
                Console.WriteLine();
            }
            catch (NpgsqlException ex)
            {
                //TODO: throw DbException;
            }

            conn.Close();
        }

        public static void Delete(Entity entity)
        {
            var conn = Connect();
            var type = entity.GetType();
            var model = new Model(type);

            string sql = DbSchema is not null ? $"DELETE FROM {DbSchema}.{model.TableName} WHERE id=@id" : $"DELETE FROM {model.TableName} WHERE id=@id";
            
            var cmd = conn.CreateCommand();
            
            var pk = model.Fields.First(x => x.IsPrimaryKey);
            var pkValue = pk.GetValue(entity);
            cmd.AddParameter("@id", pkValue);
            
            cmd.CommandText = sql;

            try
            {
                var success = cmd.ExecuteScalar();
                Console.WriteLine();
            }
            catch (NpgsqlException ex)
            {
                //TODO: throw DbException;
            }

            conn.Close();
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