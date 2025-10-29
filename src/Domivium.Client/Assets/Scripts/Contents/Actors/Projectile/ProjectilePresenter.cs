using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using Domivium.Client.Core.Factory;

namespace Domivium.Client.Contents.Actors.Projectile
{
    public abstract class ProjectilePresenter<TProjectile> : ActorPresenter<TProjectile>, IPawnPresenter where TProjectile : Projectile
    {
        public IBattleSystem BattleSystem { get; }

        protected ProjectilePresenter(TProjectile actor, ISystemFactory systemFactory)
            : base(actor, systemFactory) { }
    }
}