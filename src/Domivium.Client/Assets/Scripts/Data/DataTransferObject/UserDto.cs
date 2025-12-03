using System;

namespace Domivium.Client.Data.DataTransferObject
{
    [Serializable]
    public class UserDto
    {
        public int Id;
        public int CharacterId;
        public int Level;
        public int Experience;
        public DateTime Modified;
    }
}