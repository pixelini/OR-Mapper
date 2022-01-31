namespace OR_Mapper.Framework.FluentApi.Interfaces
{
    public interface IBegin<T>
    {
        /// <summary>
        /// Adds WHERE command to sql string.
        /// </summary>
        /// <param name="column">Column that is used in query.</param>
        /// <returns>Current object instance.</returns>
        public IWhereClause<T> Where(string column);

        /// <summary>
        /// Adds MAX command to sql string.
        /// </summary>
        /// <param name="column">Column that is used in query.</param>
        /// <returns>Current object instance.</returns>
        public IMax<T> Max(string column);
        
        /// <summary>
        /// Adds MIN command to sql string.
        /// </summary>
        /// <param name="column">Column that is used in query.</param>
        /// <returns>Current object instance.</returns>
        public IMin<T> Min(string column);

        /// <summary>
        /// Adds AVG command to sql string.
        /// </summary>
        /// <param name="column">Column that is used in query.</param>
        /// <returns>Current object instance.</returns>
        public IAvg<T> Avg(string column);
    }
}