namespace Domivium.Shared.Common
{
    public static class StatusCode
    {
        public const int Success = 0;

        // Auth/User
        public const int InvalidCredentials = 1001;
        public const int UserNotFound = 1002;
        public const int UserAlreadyExist = 1003;
        public const int Unauthenticated = 1004;
        public const int UserAlreadyExists = 1005;

        // System
        public const int InternalServerError = 5000;
    }
}