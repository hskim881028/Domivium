namespace Domivium.Client.Contents.Actors
{
    public class Character : Unit
    {
        public bool IsRemainingDistance()
        {
            if (_agent.pathPending) return false;

            return _agent.remainingDistance > _agent.stoppingDistance;
        }
    }
}