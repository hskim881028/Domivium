using System;
using System.Threading;
using Domivium.Client.Data.SecureStore;

namespace Domivium.Client.Data.Cache
{
    public sealed class AuthenticationTokenCache
    {
        private sealed class Snapshot
        {
            public readonly string AccessToken;
            public readonly string RefreshToken;
            public readonly DateTimeOffset AccessExp;
            public readonly DateTimeOffset RefreshExp;

            public Snapshot(string accessToken, string refreshToken, DateTimeOffset accessExp, DateTimeOffset refreshExp)
            {
                AccessToken = accessToken;
                RefreshToken = refreshToken;
                AccessExp = accessExp;
                RefreshExp = refreshExp;
            }

            public static readonly Snapshot Empty = new(string.Empty, string.Empty, DateTimeOffset.MinValue, DateTimeOffset.MinValue);
        }

        private static readonly TimeSpan DefaultSkew = TimeSpan.FromSeconds(10);
        private const string RefreshTokenKey = "auth.refreshToken";
        private readonly ISecureStore _secureStore;
        private Snapshot _snapshot;

        public AuthenticationTokenCache(ISecureStore secureStore)
        {
            _secureStore = secureStore;
            _snapshot = new Snapshot(
                string.Empty,
                _secureStore.GetString(RefreshTokenKey, string.Empty),
                DateTimeOffset.MinValue,
                DateTimeOffset.MinValue);
        }

        public bool IsAccessTokenExpired => IsAccessTokenExpiredWithSkew(DefaultSkew);
        public bool IsRefreshTokenExpired => IsRefreshTokenExpiredWithSkew(DefaultSkew);
        public string AccessToken => Volatile.Read(ref _snapshot).AccessToken;
        public string RefreshToken => Volatile.Read(ref _snapshot).RefreshToken;

        public void Update(string accessToken, string refreshToken, int accessTokenLifetimeSeconds, int refreshTokenLifetimeSeconds)
        {
            var now = DateTimeOffset.UtcNow;

            if (!string.IsNullOrEmpty(refreshToken))
            {
                _secureStore.SetString(RefreshTokenKey, refreshToken);
            }

            var next = new Snapshot(
                accessToken ?? string.Empty,
                refreshToken ?? string.Empty,
                now.AddSeconds(accessTokenLifetimeSeconds),
                now.AddSeconds(refreshTokenLifetimeSeconds));

            Interlocked.Exchange(ref _snapshot, next);
        }

        public void Clear()
        {
            _secureStore.Delete(RefreshTokenKey);
            Interlocked.Exchange(ref _snapshot, Snapshot.Empty);
        }

        private bool IsAccessTokenExpiredWithSkew(TimeSpan skew)
        {
            var s = Volatile.Read(ref _snapshot);
            return string.IsNullOrEmpty(s.AccessToken) || s.AccessExp <= DateTimeOffset.UtcNow + skew;
        }

        private bool IsRefreshTokenExpiredWithSkew(TimeSpan skew)
        {
            var s = Volatile.Read(ref _snapshot);
            return string.IsNullOrEmpty(s.RefreshToken) || s.RefreshExp <= DateTimeOffset.UtcNow + skew;
        }
    }
}