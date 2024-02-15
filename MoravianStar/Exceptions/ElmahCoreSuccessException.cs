using System;
using System.Runtime.Serialization;

namespace MoravianStar.Exceptions
{
    public class ElmahCoreSuccessException : Exception
    {
        public ElmahCoreSuccessException()
        {
        }

        public ElmahCoreSuccessException(string message) : base(message)
        {
        }

        public ElmahCoreSuccessException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if NET8_0_OR_GREATER
        [Obsolete(DiagnosticId = "SYSLIB0051")]
#endif
        protected ElmahCoreSuccessException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}