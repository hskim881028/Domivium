using Domivium.Client.Core.Context;
using Domivium.Client.Data.DataTransferObject;
using Domivium.Client.Data.User;
using R3;

namespace Domivium.Client.Contents.Context
{
    public sealed class UserContext : IUserContext
    {
        private readonly User _user = new();
        private readonly ReactiveProperty<int> _level = new();
        private readonly ReactiveProperty<int> _filledExperience = new();
        private readonly ReactiveProperty<int> _experience = new();

        public int Id => _user.Id;
        public int CharacterId => _user.CharacterId;
        public ReadOnlyReactiveProperty<int> Level => _level;
        public ReadOnlyReactiveProperty<int> FilledExperience => _filledExperience;
        public ReadOnlyReactiveProperty<int> Experience => _experience;

        public void Initialize(UserDto data)
        {
            _user.SetData(data);
            _level.Value = _user.Level;
            _filledExperience.Value = _user.Experience;
            _experience.Value = _user.Level * 100;
        }

        public bool TryToDto(out UserDto data) => _user.ToDto(out data);
    }
}