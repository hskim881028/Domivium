using System.Collections.Generic;

namespace Domivium.Client.Core.UI.Navigation
{
    public sealed class UINavigationNodePool : Disposable, IUINavigationNodePool
    {
        private readonly Queue<IUINavigationNode> _pool = new();

        public IUINavigationNode Get(UIId id)
        {
            if (_pool.TryDequeue(out var node))
            {
                node.Reset(id);
                return node;
            }

            var newNode = new UINavigationNode(id, UIHandle.Create);
            return newNode;
        }

        public void Return(IUINavigationNode node)
        {
            _pool.Enqueue(node);
        }

        protected override void OnDispose()
        {
            foreach (var node in _pool)
            {
                node.Dispose();
            }

            _pool.Clear();
            base.OnDispose();
        }
    }
}