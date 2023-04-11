using System;
using System.Runtime.Serialization;

namespace MoravianStar.Exceptions
{
    /// <summary>
    /// The exception that is thrown when trying to create or update an entity which conflicts with another entity by uniqueness.
    /// </summary>
    public class EntityNotUniqueException : Exception
    {
        public EntityNotUniqueException()
        {
        }

        public EntityNotUniqueException(string message) : base(message)
        {
        }

        public EntityNotUniqueException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EntityNotUniqueException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}