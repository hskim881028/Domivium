using MasterMemory;
using MessagePack;

namespace Domivium.Client.Data.Row
{
    [MemoryTable("wave"), MessagePackObject(true)]
    public class WaveRow
    {
        [PrimaryKey, NonUnique] public int StageId { get; set; }
        public int WaveId { get; set; }
        public int CampIndex { get; set; }
        public int MonsterId { get; set; }
        public float SpawnTime { get; set; }
    }
}