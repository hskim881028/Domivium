#if UNITY_ANDROID
namespace Domivium.Client.Data.SecureStore
{
    public class AndroidSecureStore : ISecureStore
    {
        public void SetString(string key, string value) { }

        public string GetString(string key, string defaultValue = "") => string.Empty;

        public void Delete(string key) { }
    }
}
#endif