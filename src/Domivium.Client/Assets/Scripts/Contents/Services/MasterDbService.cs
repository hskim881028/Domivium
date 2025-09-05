using UnityEngine;

namespace Domivium.Client.Contents.Services
{
    public sealed class MasterDbService
    {
        public MemoryDatabase DB { get; }

        public MasterDbService(TextAsset masterDb)
        {
            DB = new MemoryDatabase(masterDb.bytes);
        }
    }
}