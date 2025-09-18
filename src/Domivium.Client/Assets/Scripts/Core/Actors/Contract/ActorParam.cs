using UnityEngine;

namespace Domivium.Client.Core.Actors.Contract
{
    public record ActorParam
    {
        public static ActorParam Empty => new();
    }

    public static class ActorParamExtensions
    {
        public static T As<T>(this ActorParam param) where T : ActorParam
        {
            if (param is T casted) return casted;

            Debug.LogError($"{param.GetType().Name} is not {typeof(T).Name}");
            return null;
        }
    }
}