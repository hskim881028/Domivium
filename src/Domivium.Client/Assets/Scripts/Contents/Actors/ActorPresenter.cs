using System.Threading;
using Cysharp.Threading.Tasks;
using Domivium.Client.Contents.State;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Actors.Contract;
using Domivium.Client.Core.Factory;
using Domivium.Client.Core.State;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public abstract class ActorPresenter<TActor> : Disposable, IActorPresenter where TActor : Actor
    {
        protected TActor Actor { get; }
        protected IStateSystem StateSystem { get; }

        public ushort Uid { get; private set; }
        public ActorId ActorId { get; private set; }
        public Transform Transform => Actor.transform;

        protected ActorPresenter(TActor actor, ISystemFactory systemFactory)
        {
            Actor = actor;
            Actor.Configure(systemFactory.SpriteSystem);
            StateSystem = systemFactory.CreateState(this);
            StateSystem.Tag.DistinctUntilChanged().Subscribe(OnStateChanged).AddTo(ref DisposableBag);
        }

        public virtual void Initialize(ushort uid, ActorId actorId, Transform parent)
        {
            Uid = uid;
            ActorId = actorId;
            Actor.Initialize(uid, actorId, parent);
        }

        public virtual async UniTask SpawnAsync(CancellationToken token, ActorParam param)
        {
            await Actor.SpawnAsync(token, param);
        }

        public virtual void Activate() { }

        public virtual void Despawn()
        {
            Actor.Despawn();
        }

        public virtual void Tick(float deltaTime) // Die, Despawn 상태가 되면 Actor Manager에서 remove됨
        {
            StateTick(deltaTime);
            Actor.Tick(deltaTime);
        }

        public virtual bool CanTransitState(StateTag current, StateTag next) => true;

        public void Terminate()
        {
            StateSystem.Terminate();
        }

        protected override void OnDispose()
        {
            StateSystem.Dispose();
            base.OnDispose();
        }

        protected virtual void OnIdleTick() { }
        protected virtual void OnMoveTick(float deltaTime) { }
        protected virtual void OnPostStateTick(float deltaTime) { }

        protected virtual void OnIdle() { } //this.Log(Uid);

        protected virtual void OnMove() { } //this.Log(Uid);

        protected virtual void OnDie() { }
        protected virtual void OnTerminated() { } //this.Log(Uid);

        private void StateTick(float deltaTime)
        {
            if (StateSystem.Tag.CurrentValue == StateTags.Idle)
            {
                OnIdleTick();
            }
            else if (StateSystem.Tag.CurrentValue == StateTags.Move)
            {
                OnMoveTick(deltaTime);
            }

            OnPostStateTick(deltaTime);
        }

        private void OnStateChanged(StateTag tag)
        {
            if (tag == StateTags.Idle)
            {
                OnIdle();
            }
            else if (tag == StateTags.Move)
            {
                OnMove();
            }
            else if (tag == StateTags.Die)
            {
                OnDie();
            }
            else if (tag == StateTags.Terminated)
            {
                OnTerminated();
            }
        }
    }
}