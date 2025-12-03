using Domivium.Client.Data.DataTransferObject;
using R3;

namespace Domivium.Client.Core.Context
{
    public interface IUserContext
    {
        public int Id { get; }
        public int CharacterId { get; }
        public ReadOnlyReactiveProperty<int> Level { get; }
        public ReadOnlyReactiveProperty<int> FilledExperience { get; }
        public ReadOnlyReactiveProperty<int> Experience { get; }
        public void Initialize(UserDto data);
        public bool TryToDto(out UserDto data);
    }
}