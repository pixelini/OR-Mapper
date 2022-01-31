using System.Data;

namespace OR_Mapper.Framework.Extensions
{
    /// <summary>
    /// This class is used as extension class for IDbCommands in order to get add parameters.
    /// </summary>
    public static class IDbCommandExtensions
    {
        public static void AddParameter(this IDbCommand command, string name, object? value)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            command.Parameters.Add(parameter);
        }
    }
}