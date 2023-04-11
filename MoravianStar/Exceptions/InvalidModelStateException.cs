using System;
using System.Runtime.Serialization;

namespace MoravianStar.Exceptions
{
    /// <summary>
    /// The exception that is thrown when some property or collection in the model state is not valid.
    /// </summary>
    public class InvalidModelStateException : Exception
    {
        public InvalidModelStateException()
        {
        }

        public InvalidModelStateException(string message) : base(message)
        {
        }

        public InvalidModelStateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidModelStateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}