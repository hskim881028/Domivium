using UnityEngine;

namespace Domivium.Client.Contents.DI.Container
{
    [CreateAssetMenu(fileName = "ConfigContainer", menuName = "ScriptableObjects/ConfigContainer")]
    public class ConfigContainer : ScriptableObject
    {
        [SerializeField] private bool _localMode;
        [SerializeField] private string _serverUrl = "http://localhost:5000";

        public bool LocalMode => _localMode;

        public string ServerUrl => _serverUrl;
    }
}