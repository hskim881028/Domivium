namespace Domivium.Client.Data.Stat
{
    public struct Stat
    {
        private const int Percent = 100;

        public StatDomain Domain { get; }
        public int BaseValue { get; }
        public int AddValue { get; private set; }
        public int PostAddValue { get; private set; }
        public int PercentValue { get; private set; }

        public Stat(StatDomain domain, int baseValue)
        {
            Domain = domain;
            BaseValue = baseValue;
            AddValue = 0;
            PostAddValue = 0;
            PercentValue = 0;
        }

        public int Value()
        {
            long sumAdd = BaseValue + AddValue;
            var percent = Percent + PercentValue;
            var value = sumAdd * percent / Percent;
            value += PostAddValue;
            if (value < 0)
            {
                value = 0;
            }

            return (int)value;
        }

        public void Add(int value) => AddValue += value;

        public void PostAdd(int value) => PostAddValue += value;

        public void AddMultiplier(int percent) => PercentValue += percent;
        public void SetMultiplier(int percent) => PercentValue = percent;

        public void Reset()
        {
            AddValue = 0;
            PostAddValue = 0;
            PercentValue = 0;
        }
    }
}