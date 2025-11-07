using UnityEngine;

namespace Domivium.Client.Contents.DI.Container
{
    [CreateAssetMenu(fileName = "ConfigContainer", menuName = "ScriptableObjects/ConfigContainer")]
    public class ConfigContainer : ScriptableObject
    {
        [SerializeField] private bool _localMode;
        [SerializeField] private bool _drawRange;
        [SerializeField] private string _serverUrl = "http://localhost:5000";
        [SerializeField] private TextAsset _masterDB;

        public bool LocalMode => _localMode;
        public bool DrawRange => _drawRange;
        public string ServerUrl => _serverUrl;
        public TextAsset MasterDB => _masterDB;
    }
}