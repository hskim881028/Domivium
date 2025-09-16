using System.Collections.Generic;
using Domivium.Client.Core.Message;

namespace Domivium.Client.Core.Input
{
    public sealed class InputRouter : Disposable
    {
        private readonly List<IInputConsumer> _consumers = new();

        public void Register(IInputConsumer consumer)
        {
            _consumers.Add(consumer);
            Sort();
        }

        public void Unregister(IInputConsumer consumer)
        {
            _consumers.Remove(consumer);
            Sort();
        }

        public void OnInput(InputMessage message)
        {
            foreach (var consumer in _consumers)
            {
                if (consumer.TryHandle(message))
                {
                    break;
                }
            }
        }

        protected override void OnDispose()
        {
            _consumers.Clear();
        }

        private void Sort()
        {
            _consumers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
        }
    }
}