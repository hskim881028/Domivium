using UnitGenerator;

namespace Domivium.Client.Core.Audio
{
    [UnitOf(typeof(string))]
    public readonly partial struct AudioParam
    {
        public static AudioParam Master = new("Master");
        public static AudioParam BGM = new("BGM");
        public static AudioParam SFX = new("SFX");
        public static AudioParam UI = new("UI");
    }
}