using Domivium.Client.Core.Battle;

namespace Domivium.Client.Core.Message
{
    public readonly struct BattleCueMessage
    {
        public BattleCueId CueId { get; }
        public BattleCueContext Context { get; }

        private BattleCueMessage(BattleCueId cueId, in BattleCueContext context)
        {
            CueId = cueId;
            Context = context;
        }

        public static BattleCueMessage Emit(BattleCueId cueId, in BattleCueContext context) => new(cueId, context);
    }
}