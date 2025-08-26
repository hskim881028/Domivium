using System;

namespace Domivium.Shared.Exceptions
{
    public class ServerExceptions : Exception
    {
        public int StatusCode { get; private set; }

        public ServerExceptions(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}