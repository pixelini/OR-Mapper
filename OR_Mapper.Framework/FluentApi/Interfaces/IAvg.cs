namespace OR_Mapper.Framework.FluentApi.Interfaces
{
    public interface IAvg<T>
    {
        /// <summary>
        /// Executes the AVG sql command and loads data.
        /// </summary>
        /// <returns>Queried result value.</returns>
        public object Execute();
    }
}