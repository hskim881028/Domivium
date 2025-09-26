namespace Domivium.Client.Contents.Controller
{
    public interface IWaveController
    {
        public void Initialize(int stageId);
        public void Tick(float deltaTime);
    }
}