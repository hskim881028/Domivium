using System;
using System.Collections.Generic;
using Domivium.Client.Core.Message;

namespace Domivium.Client.Core.Input
{
    public sealed class InputRouter : IDisposable
    {
        private readonly List<IInputConsumer> _consumers = new();
        private bool _isDisposed;

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

        private void Sort()
        {
            _consumers.Sort((a, b) => a.Priority.CompareTo(b.Priority));
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

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _consumers.Clear();
        }
    }
}