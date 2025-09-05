using Domivium.Client;
using MessagePack;
using MessagePack.Resolvers;
using UnityEngine;

// ReSharper disable CheckNamespace
public static class MessagePackBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    public static void Setup()
    {
        StaticCompositeResolver.Instance.Register(MasterMemoryResolver.Instance, StandardResolver.Instance);
        var options = MessagePackSerializerOptions.Standard.WithResolver(StaticCompositeResolver.Instance);
        MessagePackSerializer.DefaultOptions = options;
    }
}