using System;

namespace RimDev.Supurlative
{
    public class FormatterException : Exception
    {
        public FormatterException(
            string message,
            Exception innerException)
            : base(message, innerException)
        { }
    }
}
