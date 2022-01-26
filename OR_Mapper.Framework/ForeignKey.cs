namespace OR_Mapper.Framework
{
    public class ForeignKey
    {
        public Field LocalColumn { get; set; }  // fk_PersonId
        
        public Field ForeignColumn { get; set; }  // PersonId

        public Model ForeignTable { get; set; }

        public ForeignKey(Field localColumn, Field foreignColumn, Model foreignTable)
        {
            LocalColumn = localColumn;
            ForeignColumn = foreignColumn;
            ForeignTable = foreignTable;
        }
    }
}