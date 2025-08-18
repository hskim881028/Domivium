using System;

namespace Domivium.Client.Core.Exceptions
{
    public class InitializationFailedException : Exception
    {
        public InitializationFailedException(string message, Exception innerException)
            : base(message, innerException) { }
    }

    public class AlreadyOpenedException : Exception { }
}