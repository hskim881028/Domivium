namespace Domivium.Client.Core.Component
{
    public class CostButton : DvmButton
    {
        private CostText _cost;

        public int Cost
        {
            get => _cost.Cost;
            set => _cost.Cost = value;
        }

        protected override void Awake()
        {
            base.Awake();
            _cost = GetComponentInChildren<CostText>();
        }
    }
}