using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Input;
using Npgsql;
using OR_Mapper.Framework.Database;
using OR_Mapper.Framework.Extensions;

namespace OR_Mapper.Framework.FluentApi
{
    public interface IBegin<T>
    {
        public IWhereClause<T> Where(string column);

        public IMax<T> Max(string column);
        
        public IMin<T> Min(string column);

        public IAvg<T> Avg(string column);

    }

    public interface IMax<T>
    {
        public object Execute();
    }
    
    public interface IMin<T>
    {
        public object Execute();
    }
    
    public interface IAvg<T>
    {
        public object Execute();
    }

    public interface IWhereClause<T>
    {
        public IWhere<T> Is(object? value);
        
        public IWhere<T> IsGreaterThan(double value);
        
        public IWhere<T> IsLessThan(double value);
        
        public IWhere<T> IsGreaterOrEqual(double value);
        
        public IWhere<T> IsLessOrEqual(double value);
    }

    public interface IWhere<T>
    {
        public IWhereClause<T> And(string column);

        public List<T> Execute();
    }
    
    public class FluentApi
    {
        private static IDbConnection _connection;

        public FluentApi(string connectionString)
        {
            _connection = new NpgsqlConnection(connectionString);
        }

        public static void UseConnection(Func<IDbConnection> funcConnection)
        {
            _connection = funcConnection();
        }

        public static IBegin<T> Entity<T>() where T : new()
        {
            return new FluentBuilder<T>();
        }

        private class FluentBuilder<T> : IBegin<T>, IWhere<T>, IWhereClause<T>, IMax<T>, IMin<T>, IAvg<T> where T : new()
        {
            private IDbCommand _cmd { get; set; }
            
            private string _sql { get; set; }
            
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
                    //TODO: throw ColumnNotFoundException();
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
                    //TODO: throw ColumnNotFoundException();
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
                    //TODO: throw ColumnNotFoundException();
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
                    //TODO: throw ColumnNotFoundException();
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
                    //TODO: throw ColumnNotFoundException();
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

            public IWhere<T> IsGreaterOrEqual(double value)
            {
                _paramCounter++;
                _sql += $" >= @p{_paramCounter}";
                _cmd.AddParameter($"@p{_paramCounter}", value);
                return this;
            }

            public IWhere<T> IsLessOrEqual(double value)
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
                    //TODO: throw DbException;
                    throw;
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
                    //TODO: throw DbException;
                    throw;
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
                    //TODO: throw DbException;
                    throw;
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
                    //TODO: throw DbException;
                    throw;
                }
            }
        }
    }
}