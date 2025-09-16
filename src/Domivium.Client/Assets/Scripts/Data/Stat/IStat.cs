namespace Domivium.Client.Data.Stat
{
    public interface IStat
    {
        public StatId Id { get; }
        public void Clear();
        public int Value();
    }
}