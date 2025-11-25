using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Domivium.Client.Core.Exceptions;
using Domivium.Client.Core.Message;
using Domivium.Client.Core.UI.Contract;
using Domivium.Client.Core.UI.Presenter;
using MessagePipe;
using R3;

namespace Domivium.Client.Core.UI.Navigation
{
    public sealed class UINavigation : Disposable, IUINavigation
    {
        private readonly IUINavigationNodePool _navigationNodePool;
        private readonly IUIManager _uiManager;
        private readonly Stack<IUINavigationNode> _stackNodes = new();
        private readonly HashSet<IUINavigationNode> _staticNodes = new();
        private readonly HashSet<IUINavigationNode> _systemNodes = new();

        public UINavigation(
            IUINavigationNodePool navigationNodePool,
            IUIManager uiManager,
            ISubscriber<SceneMessage> sceneSubscriber)
        {
            _navigationNodePool = navigationNodePool;
            _uiManager = uiManager;
            sceneSubscriber.Subscribe(OnSceneMessage).AddTo(ref DisposableBag);
        }

        public bool IsRunning { get; private set; }
        public bool HasOpenSystemUI => _systemNodes.Count > 0;
        public bool HasOpenStackUI => _stackNodes.Count > 0;

        public bool IsTopOfStack(UIId id)
        {
            if (!_stackNodes.TryPeek(out var node)) return false;

            return node.Id == id;
        }

        public async UniTask ApplyUILayer(UILayer layer, bool immediately = false)
        {
            try
            {
                var tasks = new List<UniTask>();
                var uiIdsByLayer = _uiManager.GetStaticUI(layer);
                foreach (var node in _staticNodes)
                {
                    var presenter = _uiManager.Get<IStaticUIPresenter>(node.Id);
                    tasks.Add(HidePresenterAsync(presenter, node, UIResult.Close, immediately));
                    _navigationNodePool.Return(node);
                }
                _staticNodes.Clear();

                foreach (var uiId in uiIdsByLayer)
                {
                    var presenter = _uiManager.Get<IStaticUIPresenter>(uiId);
                    var node = _navigationNodePool.Get(uiId);
                    _staticNodes.Add(node);
                    tasks.Add(ShowPresenterAsync(presenter, node, UIParam.Empty, immediately));
                }

                await UniTask.WhenAll(tasks);
            }
            catch (InitializationFailedException)
            {
                foreach (var node in _staticNodes)
                {
                    _uiManager.Remove(node.Id);
                }
            }
        }

        public async UniTask<IUIHandle> ShowStackUIAsync(UIId id, UIParam payload = null, bool immediately = false)
        {
            if (IsRunning) return UIHandle.Running;

            try
            {
                if (_stackNodes.Any(x => x.Id == id))
                {
                    throw new AlreadyOpenedException();
                }

                IsRunning = true;
                var node = _navigationNodePool.Get(id);
                var presenter = _uiManager.Get<IStackUIPresenter>(id);
                var handle = await ShowPresenterAsync(presenter, node, payload, immediately);
                _stackNodes.Push(node);
                IsRunning = false;
                return handle;
            }
            catch (InitializationFailedException)
            {
                _uiManager.Remove(id);
            }
            catch (AlreadyOpenedException)
            {
                return UIHandle.Error;
            }

            return UIHandle.Error;
        }

        public async UniTask<IUIHandle> ShowSystemUIAsync(UIId id, UIParam payload = null, bool immediately = false)
        {
            if (IsRunning) return UIHandle.Running;

            try
            {
                IsRunning = true;
                var node = _systemNodes.FirstOrDefault(x => x.Id == id);
                if (node == null)
                {
                    node = _navigationNodePool.Get(id);
                    _systemNodes.Add(node);
                }

                var presenter = _uiManager.Get<ISystemUIPresenter>(id);
                var handle = await ShowPresenterAsync(presenter, node, payload ?? UIParam.Empty, immediately);
                IsRunning = false;
                return handle;
            }
            catch (InitializationFailedException)
            {
                var node = _navigationNodePool.Get(id);
                _systemNodes.Remove(node);
                _uiManager.Remove(id);
            }

            return UIHandle.Error;
        }

        public async UniTask<bool> HideSystemUIAsync(UIId id, UIResult result, bool immediately = false)
        {
            if (IsRunning) return false;

            var node = _systemNodes.FirstOrDefault(x => x.Id == id);
            if (node == null)
            {
                return false;
            }

            _systemNodes.Remove(node);

            var presenter = _uiManager.Get<ISystemUIPresenter>(id);
            await HidePresenterAsync(presenter, node, result, immediately);
            _navigationNodePool.Return(node);
            return true;
        }

        public async UniTask<bool> HideStackUIAsync(UIResult result, bool immediately = false)
        {
            if (IsRunning) return false;

            if (!_stackNodes.TryPop(out var node))
            {
                return false;
            }

            var presenter = _uiManager.Get<IStackUIPresenter>(node.Id);
            await HidePresenterAsync(presenter, node, result, immediately);
            _navigationNodePool.Return(node);
            return true;
        }

        private async UniTask<IUIHandle> ShowPresenterAsync(
            IUIPresenter presenter,
            IUINavigationNode node,
            UIParam payload,
            bool immediately)
        {
            node.Reset(node.Id);
            presenter.Activate(0);
            await presenter.InitializeAsync(node.Token);
            presenter.OnShowEnter();
            await presenter.ShowAsync(node.Token, payload, immediately);
            presenter.OnShowExit();
            return node.Opened();
        }

        private async UniTask HidePresenterAsync(
            IUIPresenter presenter,
            IUINavigationNode node,
            UIResult result,
            bool immediately)
        {
            presenter.OnHideEnter();
            await presenter.HideAsync(node.Token, immediately);
            presenter.OnHideExit();
            node.Closed(result);
        }

        private void OnSceneMessage(SceneMessage message)
        {
            switch (message.Type)
            {
                case SceneMessageType.Unload:
                    foreach (var node in _stackNodes)
                    {
                        _navigationNodePool.Return(node);
                    }
                    _stackNodes.Clear();

                    foreach (var node in _staticNodes)
                    {
                        _navigationNodePool.Return(node);
                    }
                    _staticNodes.Clear();

                    foreach (var node in _systemNodes)
                    {
                        _navigationNodePool.Return(node);
                    }
                    _systemNodes.Clear();
                    break;
                case SceneMessageType.Load:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}