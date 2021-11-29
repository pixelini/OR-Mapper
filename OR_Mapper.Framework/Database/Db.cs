using System;
using System.Data;
using System.Diagnostics;
using Npgsql;

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

        // sends query to database which executes it
        public static IDataReader Query(string sql)
        {
            using var cmd = Connection.CreateCommand();
            cmd.CommandText = sql;
            cmd.Parameters.Add(new NpgsqlParameter("@id", 1));
            cmd.Parameters.Add(new NpgsqlParameter("@name", "hans"));
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
        
        
    }
}