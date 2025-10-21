using Domivium.Client.Core.Actors;
using Domivium.Client.Core.Battle;
using UnityEngine;

namespace Domivium.Client.Contents.Actors
{
    public class CharacterSelectIndicator : Actor
    {
        [SerializeField] private GameObject _render;

        private IBattleSystem _character;

        protected override void OnUpdate()
        {
            if (_character == null) return;

            transform.position = _character.UnitPosition;
            base.OnUpdate();
        }

        public void SetCharacter(IBattleSystem character)
        {
            _render.SetActive(character != null);
            _character = character;
        }
    }
}