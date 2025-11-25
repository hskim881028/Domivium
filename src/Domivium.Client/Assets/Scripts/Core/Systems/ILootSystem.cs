using R3;

namespace Domivium.Client.Core.Systems
{
    public interface ILootSystem
    {
        public ReactiveCommand<ushort> OnFind { get; }
    }
}