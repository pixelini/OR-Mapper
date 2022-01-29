namespace OR_Mapper.Framework
{
    /// <summary>
    /// This class holds local and foreign field information about foreign key.
    /// </summary>
    public class ForeignKey
    {
        public Field LocalColumn { get; set; }  // fk_PersonId
        
        public Field ForeignColumn { get; set; }  // PersonId

        public Model ForeignTable { get; set; }

        
        /// <summary>
        /// Creates a new foreign key from local and foreign information.
        /// </summary>
        /// <param name="localColumn">Local column.</param>
        /// <param name="foreignColumn">Foreign column.</param>
        /// <param name="foreignTable">Foreign table.</param>
        public ForeignKey(Field localColumn, Field foreignColumn, Model foreignTable)
        {
            LocalColumn = localColumn;
            ForeignColumn = foreignColumn;
            ForeignTable = foreignTable;
        }
    }
}