namespace Domivium.Client.Core.Actors
{
    public interface IUnit : ITicker
    {
        public void SetHealth(int current, int max);
        public void SetAttackRange(int attackRange);
        public void SetDetectionRange(float detectionRange);
    }
}