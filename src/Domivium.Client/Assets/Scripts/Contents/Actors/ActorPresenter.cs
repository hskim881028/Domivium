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

        public ushort Id { get; private set; }

        public abstract ActorId ActorId { get; }

        protected ActorPresenter(TActor actor, ISystemFactory systemFactory)
        {
            Actor = actor;
            StateSystem = systemFactory.CreateState(this);
            StateSystem.Tag.DistinctUntilChanged().Subscribe(OnStateChanged).AddTo(ref DisposableBag);
        }

        public virtual void Initialize(ushort id, Transform parent)
        {
            Id = id;
            Actor.Initialize(id, parent);
        }

        public virtual async UniTask ActivateAsync(CancellationToken token, ActorParam param)
        {
            await Actor.ActivateAsync(token, param);
        }

        public virtual void Deactivate()
        {
            Actor.Deactivate();
        }

        public virtual void Tick(float deltaTime) // Die, Despawn 상태가 되면 Actor Manager에서 remove됨
        {
            StateTick();
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
        protected virtual void OnChaseTick() { }
        protected virtual void OnBattleTick() { }
        protected virtual void OnMoveTick() { }

        protected virtual void OnIdle() => this.Log(Id);
        protected virtual void OnChase() => this.Log(Id);
        protected virtual void OnBattle() => this.Log(Id);
        protected virtual void OnMove() => this.Log(Id);
        protected virtual void OnDie() { }
        protected virtual void OnTerminated() => this.Log(Id);

        private void StateTick()
        {
            if (StateSystem.Tag.CurrentValue == StateTags.Idle)
            {
                OnIdleTick();
            }
            else if (StateSystem.Tag.CurrentValue == StateTags.Chase)
            {
                OnChaseTick();
            }
            else if (StateSystem.Tag.CurrentValue == StateTags.Battle)
            {
                OnBattleTick();
            }
            else if (StateSystem.Tag.CurrentValue == StateTags.Move)
            {
                OnMoveTick();
            }
        }

        private void OnStateChanged(StateTag tag)
        {
            if (tag == StateTags.Idle)
            {
                OnIdle();
            }
            else if (tag == StateTags.Chase)
            {
                OnChase();
            }
            else if (tag == StateTags.Battle)
            {
                OnBattle();
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