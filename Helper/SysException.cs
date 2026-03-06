using System;
using System.Runtime.Serialization;

namespace Inno.Helper
{
    [Serializable]
    internal class SysException : Exception
    {
        public SysException()
        {
        }

        public SysException(string message) : base(message)
        {
        }

        public SysException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SysException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}