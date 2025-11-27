using UnityEngine;

namespace Domivium.Client.Contents.Services
{
    public sealed class MasterDbService
    {
        public MemoryDatabase DB { get; }

        public MasterDbService(TextAsset data)
        {
            this.Log();
            DB = new MemoryDatabase(data.bytes);
        }
    }
}