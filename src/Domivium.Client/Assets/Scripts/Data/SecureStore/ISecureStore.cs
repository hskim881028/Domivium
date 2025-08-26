namespace Domivium.Client.Data.SecureStore
{
    public interface ISecureStore
    {
        public void SetString(string key, string value);
        public string GetString(string key, string defaultValue = "");
        public void Delete(string key);
    }
}