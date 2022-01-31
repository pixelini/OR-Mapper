namespace OR_Mapper.Framework.FluentApi.Interfaces
{
    public interface IWhereClause<T>
    {
        /// <summary>
        /// Adds IS comparison to sql string. 
        /// </summary>
        /// <param name="value">Value that is used to compare in where clause.</param>
        /// <returns>Current object instance.</returns>
        public IWhere<T> Is(object? value);
        
        /// <summary>
        /// Adds "greater than" comparison to sql string. 
        /// </summary>
        /// <param name="value">Value that is used to compare in where clause.</param>
        /// <returns>Current object instance.</returns>
        public IWhere<T> IsGreaterThan(double value);
        
        /// <summary>
        /// Adds "less than" comparison to sql string. 
        /// </summary>
        /// <param name="value">Value that is used to compare in where clause.</param>
        /// <returns>Current object instance.</returns>
        public IWhere<T> IsLessThan(double value);
        
        /// <summary>
        /// Adds "greater or equal than" comparison to sql string. 
        /// </summary>
        /// <param name="value">Value that is used to compare in where clause.</param>
        /// <returns>Current object instance.</returns>
        public IWhere<T> IsGreaterOrEqualThan(double value);
        
        /// <summary>
        /// Adds "less or equal than" comparison to sql string. 
        /// </summary>
        /// <param name="value">Value that is used to compare in where clause.</param>
        /// <returns>Current object instance.</returns>
        public IWhere<T> IsLessOrEqualThan(double value);
    }
}