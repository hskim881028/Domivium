using VContainer.Unity;

namespace Domivium.Client.Contents.DI.Entry
{
    public abstract class Entry : Disposable, IStartable
    {
        protected abstract void OnStart();

        public void Start()
        {
            OnStart();
        }
    }
}