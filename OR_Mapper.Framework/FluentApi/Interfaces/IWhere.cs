using System.Collections.Generic;

namespace OR_Mapper.Framework.FluentApi.Interfaces
{
    public interface IWhere<T>
    {
        /// <summary>
        /// Adds AND command to sql string. 
        /// </summary>
        /// <param name="column">Column that is used in query.</param>
        /// <returns>Current object instance.</returns>
        public IWhereClause<T> And(string column);

        /// <summary>
        /// Executes the sql command and loads data.
        /// </summary>
        /// <returns>Queried results.</returns>
        public List<T> Execute();
    }
}