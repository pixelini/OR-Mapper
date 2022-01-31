namespace OR_Mapper.Framework.FluentApi.Interfaces
{
    public interface IMin<T>
    {
        /// <summary>
        /// Executes the MIN sql command and loads data.
        /// </summary>
        /// <returns>Queried result value.</returns>
        public object Execute();
    }
}