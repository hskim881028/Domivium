using MasterMemory;
using MessagePack;

namespace Domivium.Client.Data.Row
{
    [MemoryTable("stage"), MessagePackObject(true)]
    public class StageRow
    {
        [PrimaryKey, NonUnique] public int StageId { get; set; }
        public string CampType { get; set; }
        public int CampIndex { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }
}