using System;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.Utility;
using MessagePipe;
using R3;
using DisposableBag = R3.DisposableBag;

namespace Domivium.Client.Contents.Controller
{
    public sealed class PlayerController : IPlayerController
    {
        private DisposableBag _disposable;
        private bool _isDisposed;

        public PlayerController(ISubscriber<InputMessage> subscriber)
        {
            subscriber.Subscribe(OnSceneMessage).AddTo(ref _disposable);
        }

        public void Dispose()
        {
            if (_isDisposed) return;

            _isDisposed = true;
            _disposable.Dispose();
        }

        private void OnSceneMessage(InputMessage message)
        {
            switch (message.Type)
            {
                case InputMessageType.Submit:
                    this.Log($"[{message.Type}]");
                    break;
                case InputMessageType.Cancel:
                    this.Log($"[{message.Type}]");
                    break;
                case InputMessageType.Point:
                    // this.Log($"[{message.Type}] : {message.Value}");
                    break;
                case InputMessageType.Click:
                    this.Log($"[{message.Type}] : {message.Value}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}