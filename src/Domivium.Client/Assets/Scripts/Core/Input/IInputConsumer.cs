using Domivium.Client.Core.Message;

namespace Domivium.Client.Core.Input
{
    public interface IInputConsumer
    {
        public InputPriority Priority { get; }
        public bool TryHandle(InputMessage message);
    }
}