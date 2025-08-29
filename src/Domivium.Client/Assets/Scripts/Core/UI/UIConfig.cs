using Domivium.Client.Core.UI.Presenter;

namespace Domivium.Client.Core.UI
{
    public static class UIConfig
    {
        public const string Static = "Static";
        public const string Stack = "Stack";
        public const string System = "System";
        public const string UIMapping = "UIMapping";
        public const string UIIds = "UIIds";

        public static string AsActor(this string str) => str.Replace("Presenter", string.Empty);

        public static string AsUI(this string str) => str
            .Replace("Presenter", string.Empty)
            .Replace($"{Static}UI", string.Empty)
            .Replace($"{Stack}UI", string.Empty)
            .Replace($"{System}UI", string.Empty);

        public static string AsCanvas(this string str)
        {
            var result = str
                .Replace(nameof(IStaticUIPresenter), Static)
                .Replace(nameof(IStackUIPresenter), Stack)
                .Replace(nameof(ISystemUIPresenter), System);
            return $"Canvas({result})";
        }
    }
}