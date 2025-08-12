namespace Domivium.Shared.Common
{
    public static class StatusCode
    {
        public const int Success = 0;

        // Auth/User
        public const int InvalidCredentials = 1001;
        public const int UserNotFound = 1002;

        // System
        public const int InternalServerError = 5000;
    }
}