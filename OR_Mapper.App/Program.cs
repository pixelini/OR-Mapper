using System;
using System.Diagnostics;
using Npgsql;
using OR_Mapper.App.Show;
using OR_Mapper.Framework.Database;

namespace OR_Mapper.App
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            // config
            Db.Connection = new NpgsqlConnection("Host=localhost;Username=postgres;Password=postgres;Database=postgres");
            Db.Connection.Open();
            //var result = Db.Query("INSERT INTO swe3_orm.person (id, name) VALUES (1, 'hans')");
            var result = Db.Fetch("select * from swe3_orm.person");

            Console.WriteLine(result);
            
            //InsertObject.Show();
            
            /*
            var sql = "INSERT INTO swe3_orm.users (id, name) VALUES (@id, @name)";
            using var cmd = new NpgsqlCommand(sql, conn);
            cmd.Parameters.Add(new NpgsqlParameter("@id", 1));
            cmd.Parameters.Add(new NpgsqlParameter("@name", "hans"));
            cmd.Prepare();
            
            try
            {
                if (cmd.ExecuteNonQuery() == 1)
                {
                    Console.WriteLine("Successful.");
                }
            }
            catch (NpgsqlException ex)
            {
                Debug.WriteLine("NpgsqlException Error Message ex.Message: " + ex.Message);
            }
            
            conn.Close();
            
            */

            
            
            //var builder = new CommandBuilder();
            

            
        }
    }
}
