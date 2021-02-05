using System;

namespace RequestProcessor.App.Exceptions
{
    /// <summary>
    /// Common options access exception.
    /// </summary>
    [Serializable]
    internal class OptionsAccessException : Exception
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public OptionsAccessException()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception details.</param>
        public OptionsAccessException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="message">Exception details.</param>
        /// <param name="innerException">Inner exception.</param>
        public OptionsAccessException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}