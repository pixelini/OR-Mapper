using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Npgsql;
using OR_Mapper.Framework.Database;
using OR_Mapper.Framework.Exceptions;
using OR_Mapper.Framework.Extensions;
using OR_Mapper.Framework.FluentApi.Interfaces;

namespace OR_Mapper.Framework.FluentApi
{
    /// <summary>
    /// This class provides an API for building db queries in a fluent manner.
    /// </summary>
    public class FluentApi
    {
        /// <summary>Holds the database connection used by the framework.</summary>
        private static IDbConnection _connection;

        
        /// <summary>
        /// Creates a new fluent API by a given db connection string.
        /// </summary>
        /// <param name="connectionString">Database connection string.</param>
        public FluentApi(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }
        
        /// <summary>
        /// Specifies the Db connection that should be used.
        /// </summary>
        /// <param name="funcConnection">Db connection string.</param>
        public static void UseConnection(Func<IDbConnection> funcConnection)
        {
            _connection = funcConnection();
        }

        /// <summary>
        /// Declares which entity type is queried.
        /// </summary>
        /// <typeparam name="T">Class of given entity.</typeparam>
        /// <returns>FluentBilder object.</returns>
        public static IBegin<T> Entity<T>() where T : new()
        {
            return new FluentBuilder<T>();
        }

        /// <summary>
        /// This class is used internally to correctly assemble the query string. The interfaces that the class implement
        /// help to create the generalised query options considering the order.
        /// </summary>
        /// <typeparam name="T">Class of given entity.</typeparam>
        private class FluentBuilder<T> : IBegin<T>, IWhere<T>, IWhereClause<T>, IMax<T>, IMin<T>, IAvg<T> where T : new()
        {
            /// <summary>
            /// Holds the Db command.
            /// </summary>
            private IDbCommand _cmd { get; set; }
            
            /// <summary>
            /// Holds the sql string.
            /// </summary>
            private string _sql { get; set; }
            
            /// <summary>
            /// Holds the parameter counter which is used to add parameters correctly to the Db command.
            /// </summary>
            private int _paramCounter { get; set; }
            
            
            public IWhereClause<T> Where(string column)
            {
                _connection.Open();
                _cmd = _connection.CreateCommand();
                var model = new Model(typeof(T));
                _sql += $"SELECT * FROM {Db.GetTableName(model.TableName)} WHERE ";

                var field = model.Fields.FirstOrDefault(x => (x.PropertyInfo?.Name ?? x.ColumnName).ToLower() == column.ToLower());
                if (field is null)
                {
                    throw new ColumnNotFoundException("Column could not be found.");
                }

                _sql += column;
                return this;
            }
            
            public IMax<T> Max(string column)
            {
                _connection.Open();
                _cmd = _connection.CreateCommand();
                var model = new Model(typeof(T));
                var field = model.Fields.FirstOrDefault(x => (x.PropertyInfo?.Name ?? x.ColumnName).ToLower() == column.ToLower());
                if (field is null)
                {
                    throw new ColumnNotFoundException("Column could not be found.");
                }
                
                _sql = $"SELECT MAX({field.ColumnName}) FROM {Db.GetTableName(model.TableName)}";
                return this;
            }
            
            public IMin<T> Min(string column)
            {
                _connection.Open();
                _cmd = _connection.CreateCommand();
                var model = new Model(typeof(T));
                var field = model.Fields.FirstOrDefault(x => (x.PropertyInfo?.Name ?? x.ColumnName).ToLower() == column.ToLower());
                if (field is null)
                {
                    throw new ColumnNotFoundException("Column could not be found.");
                }
                
                _sql = $"SELECT MIN({field.ColumnName}) FROM {Db.GetTableName(model.TableName)}";
                return this;
            }
            
            public IAvg<T> Avg(string column)
            {
                _connection.Open();
                _cmd = _connection.CreateCommand();
                var model = new Model(typeof(T));
                var field = model.Fields.FirstOrDefault(x => (x.PropertyInfo?.Name ?? x.ColumnName).ToLower() == column.ToLower());
                if (field is null)
                {
                    throw new ColumnNotFoundException("Column could not be found.");
                }
                
                _sql = $"SELECT AVG({field.ColumnName}) FROM {Db.GetTableName(model.TableName)}";
                return this;
            }
            
            public IWhereClause<T> And(string column)
            {
                var model = new Model(typeof(T));
                var field = model.Fields.FirstOrDefault(x => (x.PropertyInfo?.Name ?? x.ColumnName).ToLower() == column.ToLower());
                if (field is null)
                {
                    throw new ColumnNotFoundException("Column could not be found.");
                }
                _sql += $" AND {field.ColumnName}";
                return this;
            }
            
            public IWhere<T> Is(object? value)
            {
                if (value is null)
                {
                    _sql += " is null";
                }
                else
                {
                    _paramCounter++;
                    _sql += $" = @p{_paramCounter}";
                    _cmd.AddParameter($"@p{_paramCounter}", value);
                }
                
                return this;
            }
            
            public IWhere<T> IsGreaterThan(double value)
            {
                _paramCounter++;
                _sql += $" > @p{_paramCounter}";
                _cmd.AddParameter($"@p{_paramCounter}", value);
                return this;
            }
            
            public IWhere<T> IsLessThan(double value)
            {
                _paramCounter++;
                _sql += $" < @p{_paramCounter}";
                _cmd.AddParameter($"@p{_paramCounter}", value);
                return this;
            }
            
            public IWhere<T> IsGreaterOrEqualThan(double value)
            {
                _paramCounter++;
                _sql += $" >= @p{_paramCounter}";
                _cmd.AddParameter($"@p{_paramCounter}", value);
                return this;
            }
            
            public IWhere<T> IsLessOrEqualThan(double value)
            {
                _paramCounter++;
                _sql += $" <= @p{_paramCounter}";
                _cmd.AddParameter($"@p{_paramCounter}", value);
                return this;
            }
            
            public List<T> Execute()
            {
                // Execute command
                try
                {
                    _cmd.CommandText = _sql;
                    var reader = _cmd.ExecuteReader();
                    var loader = new ObjectLoader(reader);
                    var entityList = loader.LoadCollection<T>();
                    _connection.Close();
                    _paramCounter = 0;
                    _sql = "";
                    return entityList;
                }
                catch (NpgsqlException ex)
                {
                    throw new DatabaseException("A database error occurred, see inner exception for details.", ex);
                }
            }
            
            object IMax<T>.Execute()
            {
                // Execute command
                try
                {
                    _cmd.CommandText = _sql;
                    var reader = _cmd.ExecuteReader();
                    reader.Read();
                    var value = reader.GetValue(0);
                    _connection.Close();
                    _paramCounter = 0;
                    _sql = "";
                    return value;
                }
                catch (NpgsqlException ex)
                {
                    throw new DatabaseException("A database error occurred, see inner exception for details.", ex);
                }
            }
            
            object IMin<T>.Execute()
            {
                // Execute command
                try
                {
                    _cmd.CommandText = _sql;
                    var reader = _cmd.ExecuteReader();
                    reader.Read();
                    var value = reader.GetValue(0);
                    _connection.Close();
                    _paramCounter = 0;
                    _sql = "";
                    return value;
                }
                catch (NpgsqlException ex)
                {
                    throw new DatabaseException("A database error occurred, see inner exception for details.", ex);
                }
            }
            
            object IAvg<T>.Execute()
            {
                // Execute command
                try
                {
                    _cmd.CommandText = _sql;
                    var reader = _cmd.ExecuteReader();
                    reader.Read();
                    var value = reader.GetValue(0);
                    _connection.Close();
                    _paramCounter = 0;
                    _sql = "";
                    return value;
                }
                catch (NpgsqlException ex)
                {
                    throw new DatabaseException("A database error occurred, see inner exception for details.", ex);
                }
            }
        }
    }

}