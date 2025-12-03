using System;
using Domivium.Client.Data.DataTransferObject;

namespace Domivium.Client.Data.User
{
    public sealed class User
    {
        private bool _confirmed;

        public int Id { get; private set; }
        public int CharacterId { get; private set; }
        public int Level { get; private set; }
        public int Experience { get; private set; }

        public void SetData(UserDto data)
        {
            _confirmed = true;
            Id = data.Id;
            CharacterId = data.CharacterId;
            Level = data.Level;
            Experience = data.Experience;
        }

        public int LevelUp()
        {
            Level += 1;

            return Level;
        }

        public bool ToDto(out UserDto data)
        {
            data = new UserDto();

            if (!_confirmed) return false;

            data.Modified = DateTime.UtcNow;
            data.Id = Id;
            data.CharacterId = CharacterId;
            data.Level = Level;
            data.Experience = Experience;
            return true;
        }
    }
}