using System;
using Domivium.Shared.Services;
using MagicOnion;
using MagicOnion.Client;
using UnityEngine;

public class Tester : MonoBehaviour
{
    async void Start()
    {
        try
        {
            var channel = GrpcChannelx.ForAddress("http://localhost:5000");
            var client = MagicOnionClient.Create<ILoginService>(channel);

            var result = await client.Login();
            Debug.Log($"[StatusCode]: {result.StatusCode}");
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
