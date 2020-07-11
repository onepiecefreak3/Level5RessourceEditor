using Komponent.IO.Attributes;

namespace Leve5RessourceEditor.Level5.Models
{
    class ResStringPointer
    {
        public uint crc32;
        public short offset;
        public short unk1;
    }

    class ResPoint
    {
        public float x;
        public float y;
    }

    class ResImageEntry
    {
        public ResStringPointer stringPointer;
        public int unk1;
        public int unk2;
        public int unk3;
    }

    class ResImageArea
    {
        public ResStringPointer stringPointer;
        public int unk1;
        public uint unk2;
        public uint imageEntryParent;
        [FixedLength(0xcc)]
        public byte[] unk3;
    }

    class TableCluster2Table2
    {
        public ResStringPointer stringPointer;
        public uint unk1;
        public uint imageAreaParent;
        public uint unk3;
        public uint unk4;
        public ResPoint origin;
        public ResPoint size;
        public uint unk5;
        public uint unk6;
        public uint unk7;
        public uint unk8;
        public uint unk9;
    }

    class ResPbiDimensionEntry
    {
        public ResStringPointer stringPointer;
        public int unk1;
        public ResPoint origin;
        public ResPoint size;
        public int unk2;
    }
}
