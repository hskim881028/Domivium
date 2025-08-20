using System.Collections.Generic;

namespace Domivium.Shared.Common
{
    public static class StatusCodeMapper
    {
        private static readonly Dictionary<int, string> Messages = new()
        {
            { StatusCode.Success, "요청이 성공적으로 처리되었습니다." },
            { StatusCode.InvalidCredentials, "아이디 또는 비밀번호가 잘못되었습니다." },
            { StatusCode.UserNotFound, "사용자를 찾을 수 없습니다." },
            { StatusCode.InternalServerError, "서버 오류가 발생했습니다." }
        };

        public static string ToMessage(int code)
        {
            return Messages.TryGetValue(code, out var message) ? message : $"알 수 없는 오류 (코드: {code})";
        }
    }
}