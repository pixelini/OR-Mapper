namespace OR_Mapper.Framework.FluentApi.Interfaces
{
    public interface IMax<T>
    {
        /// <summary>
        /// Executes the MAX sql command and loads data.
        /// </summary>
        /// <returns>Queried result value.</returns>
        public object Execute();
    }
}