namespace Domivium.Client.Core.Component
{
    public class CostButton : DvmButton
    {
        private IntText _int;

        public int Cost
        {
            get => _int.Value;
            set => _int.Value = value;
        }

        protected override void Awake()
        {
            base.Awake();
            _int = GetComponentInChildren<IntText>();
        }
    }
}