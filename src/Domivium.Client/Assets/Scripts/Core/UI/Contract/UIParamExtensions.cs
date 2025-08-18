using System;

namespace Domivium.Client.Core.UI.Contract
{
    public static class UIParamExtensions
    {
        public static T As<T>(this UIParam param) where T : UIParam
        {
            if (param is T casted) return casted;

            throw new Exception($"{param.GetType().Name} is not {typeof(T).Name}");
        }
    }
}