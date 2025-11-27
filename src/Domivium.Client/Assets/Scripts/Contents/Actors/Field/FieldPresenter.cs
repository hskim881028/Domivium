using Domivium.Client.Core.Factory;
using UnityEngine.Tilemaps;

namespace Domivium.Client.Contents.Actors
{
    public abstract class FieldPresenter<TField> : ActorPresenter<TField> where TField : Field
    {
        public Tilemap ColliderGrid => Actor.ColliderGrid;

        protected FieldPresenter(TField actor, ISystemFactory systemFactory) : base(actor, systemFactory) { }
    }
}