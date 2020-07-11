using Komponent.IO.Attributes;

namespace Leve5RessourceEditor.Level5.Models
{
    class ResHeader
    {
        [FixedLength(8)]
        public string magic;

        public short stringTablesOffset;
        public short stringTablesCount;  // always 1

        public short imageTablesOffset;
        public short imageTablesCount;    // always 3

        public short tableCluster2Offset;
        public short tableCluster2Count;    // always 7

        [VariableLength(nameof(imageTablesCount))]
        public ResTableEntry[] imageTables;

        [VariableLength(nameof(tableCluster2Count))]
        public ResTableEntry[] tableCluster2;
    }
}
