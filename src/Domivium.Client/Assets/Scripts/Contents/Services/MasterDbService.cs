using UnityEngine;

namespace Domivium.Client.Contents.Services
{
    public sealed class MasterDbService
    {
        public MemoryDatabase DB { get; }

        public MasterDbService(TextAsset data)
        {
            DB = new MemoryDatabase(data.bytes);
        }
    }
}