using Domivium.Client.Core.Battle;

namespace Domivium.Client.Core.Message
{
    public readonly struct BattleCueMessage
    {
        public BattleCueId CueId { get; }
        public BattleContext Context { get; }

        private BattleCueMessage(BattleCueId cueId, in BattleContext context)
        {
            CueId = cueId;
            Context = context;
        }

        public static BattleCueMessage Emit(BattleCueId cueId, in BattleContext context) => new(cueId, context);
    }
}