using System;
using System.Runtime.Serialization;

namespace MoravianStar.Exceptions
{
    /// <summary>
    /// The exception that is thrown when a business logic error occurs.
    /// </summary>
    public class BusinessException : Exception
    {
        public BusinessException()
        {
        }

        public BusinessException(string message) : base(message)
        {
        }

        public BusinessException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
        protected BusinessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}