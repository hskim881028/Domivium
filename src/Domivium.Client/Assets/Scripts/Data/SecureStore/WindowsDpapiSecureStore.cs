#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Domivium.Client.Data.SecureStore
{
    public sealed class WindowsDpapiSecureStore : ISecureStore
    {
        private readonly string _dir;
        private readonly byte[] _entropy;

        public WindowsDpapiSecureStore()
        {
            var baseDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                Path.Combine(Application.companyName ?? "Company", Application.productName ?? "Product"));

            _dir = Path.Combine(baseDir, "SecureStore");
            Directory.CreateDirectory(_dir);
            _entropy = Encoding.UTF8.GetBytes("YourApp-ExtraEntropy-ChangeThis");
        }

        public void SetString(string key, string value)
        {
            var path = PathFor(key);
            var plain = Encoding.UTF8.GetBytes(value ?? string.Empty);
            var protectedBytes = ProtectedData.Protect(
                plain,
                _entropy,
                DataProtectionScope.CurrentUser);

            var tmp = path + ".tmp";
            File.WriteAllBytes(tmp, protectedBytes);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            File.Move(tmp, path);
        }

        public string GetString(string key, string defaultValue = "")
        {
            var path = PathFor(key);
            if (!File.Exists(path))
            {
                return defaultValue;
            }

            try
            {
                var protectedBytes = File.ReadAllBytes(path);
                var plain = ProtectedData.Unprotect(
                    protectedBytes,
                    _entropy,
                    DataProtectionScope.CurrentUser);
                return Encoding.UTF8.GetString(plain);
            }
            catch
            {
                return defaultValue;
            }
        }

        public void Delete(string key)
        {
            var path = PathFor(key);
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private string PathFor(string key)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key ?? string.Empty);
            string safe;
            using (var sha = SHA256.Create())
            {
                var hash = sha.ComputeHash(keyBytes);
                safe = Convert.ToBase64String(hash)
                    .Replace('/', '_')
                    .Replace('+', '-')
                    .TrimEnd('=');
            }
            return Path.Combine(_dir, safe + ".bin");
        }
    }
}
#endif