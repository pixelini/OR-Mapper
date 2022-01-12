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
        public static string? ConnectionString;

        public static string? DbSchema { get; set; }

        public static object? Insert(string sql)
        {
            var result = Query(sql);
            return result ?? null;
        }

        public static IDbConnection GetConnection() => new NpgsqlConnection(ConnectionString);

        // sends query to database which executes it
        public static IDataReader Query(string sql)
        {
            var connection = Connect();
            using var cmd = connection.CreateCommand();
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

            string sql = $"INSERT INTO {GetTableName(model.TableName)} (";

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

            string sql = $"DELETE FROM {GetTableName(model.TableName)} WHERE id=@id";
            
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

        public static List<TEntity> GetAll<TEntity>() where TEntity : new()
        {
            var model = new Model(typeof(TEntity));
            var conn = Connect();
            string sql = $"SELECT * FROM {GetTableName(model.TableName)}";

            var cmd = conn.CreateCommand();
            
            // Execute command
            try
            {
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                var loader = new ObjectLoader(reader);
                var entityList = loader.LoadCollection<TEntity>();
                conn.Close();
                return entityList;
            }
            catch (NpgsqlException ex)
            {
                //TODO: throw DbException;
            }
            
            return null;
        }
        
        public static TEntity? GetById<TEntity>(int id) where TEntity : new()
        {
            var model = new Model(typeof(TEntity));
            var conn = Connect();
            string sql = $"SELECT * FROM {GetTableName(model.TableName)}";
            sql += $" WHERE id = {id}";

            var cmd = conn.CreateCommand();
            
            // Execute command
            try
            {
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                var loader = new ObjectLoader(reader);
                var entity = loader.LoadSingle<TEntity>();
                conn.Close();
                return entity;
            }
            catch (NpgsqlException ex)
            {
                //TODO: throw DbException;
            }
            
            return new TEntity();
        }
        
        private static IDbConnection Connect()
        {
            try
            {
                var conn = GetConnection();
                conn.Open();
                return conn;
            }
            catch (NpgsqlException ex)
            {
                //TODO: throw DbException;
            }

            return null;
        }
        
        public static void LoadOneToOne(object record, ExternalField field) 
        {
            var conn = Connect();
            
            // Get foreign information if foreign key is in current table
            var mainModel = new Model(record.GetType());
            var foreignModel = field.Model;
            
            var columnNames = field.Model.Fields.Select(x => "v." + x.ColumnName);
            var allColumns = string.Join(',', columnNames);
            
            var fk = foreignModel.ForeignKeys.FirstOrDefault(x => x.ForeignTable.Member == mainModel.Member);
            var joinPredicate = "";

            // foreign key is in other table
            if (fk is null)
            {
                fk = mainModel.ForeignKeys.First(x => x.ForeignTable.Member == foreignModel.Member);
                joinPredicate = $"v.{foreignModel.PrimaryKey.ColumnName} = {GetTableName(mainModel.TableName)}.{fk.LocalColumn.ColumnName}";
            }
            // foreign key is in the current table
            else
            {
                joinPredicate = $"{GetTableName(mainModel.TableName)}.{mainModel.PrimaryKey.ColumnName} = v.{fk.LocalColumn.ColumnName}";
            }
            
            var sql = "";

            sql += $"SELECT {allColumns} FROM {GetTableName(mainModel.TableName)} " +
                   $"JOIN {GetTableName(foreignModel.TableName)} v " +
                   $"ON {joinPredicate} " +
                   $"WHERE {GetTableName(mainModel.TableName)}.{mainModel.PrimaryKey.ColumnName} = @p0";
                
            var cmd = conn.CreateCommand();
            cmd.AddParameter("@p0", mainModel.PrimaryKey.GetValue(record));
            
            // Execute command
            try
            {
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();
                var loader = new ObjectLoader(reader);
                var method = typeof(ObjectLoader)
                    .GetMethod(nameof(ObjectLoader.LoadSingle))?
                    .MakeGenericMethod(field.Model.Member);
                var entity = method?.Invoke(loader, new object [] {});
                conn.Close();
            }
            catch (NpgsqlException ex)
            {
                //TODO: throw DbException;
            }
            
            throw new NotImplementedException();
        }
        
        public static void LoadManyToMany(object record, ExternalField field)
        {
            throw new NotImplementedException();
        }

        public static void LoadManyToOne(object record, ExternalField field)
        {
            throw new NotImplementedException();
        }

        public static void LoadOneToMany(object record, ExternalField field)
        {
            return;
        }

        private static string GetTableName(string tableName)
        {
            return DbSchema is null ? tableName : $"{DbSchema}.{tableName}";
        }

    }
}