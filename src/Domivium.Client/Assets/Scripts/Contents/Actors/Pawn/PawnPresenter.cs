using Domivium.Client.Contents.Battle;
using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;
using R3;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public abstract class PawnPresenter<TPawn> : ActorPresenter<TPawn>, IPawnPresenter where TPawn : Pawn
    {
        public IBattleSystem BattleSystem { get; }

        protected PawnPresenter(TPawn actor, ISystemFactory systemFactory)
            : base(actor, systemFactory)
        {
            BattleSystem = systemFactory.CreateBattle(actor.transform, actor.Muzzle, actor.Collider, StateSystem.Tag);
            BattleSystem.AppliedEffect.Subscribe(OnAppliedEffectChanged).AddTo(ref DisposableBag);
            BattleSystem.Direction.Subscribe(OnDirectionChanged).AddTo(ref DisposableBag);
            BattleSystem.LookAt.Subscribe(OnLookAtChanged).AddTo(ref DisposableBag);
        }

        public override void Activate()
        {
            base.Activate();
            StateSystem.Activate();
        }

        public override void Despawn()
        {
            BattleSystem.Reset();
            base.Despawn();
        }

        public override void Tick(float deltaTime)
        {
            BattleSystem.Tick(deltaTime);
            base.Tick(deltaTime);
        }

        protected override void OnDie()
        {
            this.Log();
            Actor.Die();
            base.OnDie();
        }

        protected override void OnTerminated()
        {
            Actor.Die();
            base.OnTerminated();
        }

        protected virtual void OnDamagedEffect(BattleEffectContext context) { }

        protected virtual void OnAppliedEffectChanged(BattleEffectContext context)
        {
            if (context.EffectId == BattleEffectIds.Damage)
            {
                OnDamagedEffect(context);
            }
        }

        protected virtual void OnDirectionChanged(Vector2 direction) { }

        protected virtual void OnLookAtChanged(Vector2 lookAt)
        {
            Actor.SetFlip(lookAt.x);
        }
    }
}