using System.Runtime.CompilerServices;
using Cysharp.Text;
using Domivium.Shared.Common;
using MagicOnion.Client;
using UnityEngine;

namespace Domivium.Client.Utility
{
    public static class ZLog
    {
        public static void Request(RequestContext requestContext)
        {
#if UNITY_EDITOR
            if (!Debug.isDebugBuild) return;

            using var sb = ZString.CreateStringBuilder();
            sb.Append("<color=#4FC3F7>[Req]</color> ");
            sb.Append(requestContext.MethodPath);
            sb.Append("\n");
            sb.Append(JsonHelper.ExtractKey(requestContext, "Request"));
            Debug.Log(sb.ToString());
#endif
        }

        public static void Response(string caller, double elapsed, ResponseContext responseContext)
        {
#if UNITY_EDITOR
            if (!Debug.isDebugBuild) return;

            using var sb = ZString.CreateStringBuilder();
            sb.Append("<color=#81C784>[Res]</color> ");
            sb.Append(caller);
            sb.Append(" (Elapsed:");
            sb.Append(GetElapsedColor(elapsed));
            sb.Append(elapsed.ToString("0.0"));
            sb.Append("</color>ms)\n");
            sb.Append(JsonHelper.ExtractNestedKey(responseContext, "ResponseAsync", "Result"));
            Debug.Log(sb.ToString());
#endif
        }

        public static void StatusCodeException(int code)
        {
#if UNITY_EDITOR
            if (!Debug.isDebugBuild) return;

            using var sb = ZString.CreateStringBuilder();
            sb.Append("<color=#FF5252>[Err]</color> ");
            sb.Append(StatusCodeMapper.ToMessage(code));
            Debug.Log(sb.ToString());
#endif
        }

        private static string GetElapsedColor(double elapsed)
        {
            return elapsed <= 300 ? "<color=#81C784>" :
                elapsed <= 800 ? "<color=#FFD54F>" :
                "<color=#E57373>";
        }

        public static void Log(this object obj, string message = "", [CallerMemberName] string caller = "")
        {
#if UNITY_EDITOR
            if (!Debug.isDebugBuild) return;

            using var sb = ZString.CreateStringBuilder();
            sb.Append($"<color=#BA68C8>[Frame: {Time.frameCount}] [{obj.GetType().Name}.");
            sb.Append($"<color=#D7B2E2>{caller}</color>]</color> {message}");
            Debug.Log(sb.ToString());
#endif
        }

        public static void Log(this object obj, Vector2 value, [CallerMemberName] string caller = "")
        {
#if UNITY_EDITOR
            if (!Debug.isDebugBuild) return;

            using var sb = ZString.CreateStringBuilder();
            sb.Append($"<color=#BA68C8>[Frame: {Time.frameCount}] [{obj.GetType().Name}.");
            sb.Append($"<color=#D7B2E2>{caller}</color>]</color> {value}");
            Debug.Log(sb.ToString());
#endif
        }

        public static void Log(this object obj, ulong value, [CallerMemberName] string caller = "")
        {
#if UNITY_EDITOR
            if (!Debug.isDebugBuild) return;

            using var sb = ZString.CreateStringBuilder();
            sb.Append($"<color=#BA68C8>[Frame: {Time.frameCount}] [{obj.GetType().Name}.");
            sb.Append($"<color=#D7B2E2>{caller}</color>]</color> {value}");
            Debug.Log(sb.ToString());
#endif
        }

        public static void Log(this object obj, int value, [CallerMemberName] string caller = "")
        {
#if UNITY_EDITOR
            if (!Debug.isDebugBuild) return;

            using var sb = ZString.CreateStringBuilder();
            sb.Append($"<color=#BA68C8>[Frame: {Time.frameCount}] [{obj.GetType().Name}.");
            sb.Append($"<color=#D7B2E2>{caller}</color>]</color> {value}");
            Debug.Log(sb.ToString());
#endif
        }

        public static void Log(this object obj, bool value, [CallerMemberName] string caller = "")
        {
#if UNITY_EDITOR
            if (!Debug.isDebugBuild) return;

            using var sb = ZString.CreateStringBuilder();
            sb.Append($"<color=#BA68C8>[Frame: {Time.frameCount}] [{obj.GetType().Name}.");
            sb.Append($"<color=#D7B2E2>{caller}</color>]</color> {value}");
            Debug.Log(sb.ToString());
#endif
        }
    }
}