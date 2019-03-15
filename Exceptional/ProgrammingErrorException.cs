using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace NMaier.GetOptNet
{
  /// <inheritdoc />
  /// <summary>
  ///   Custom exception that is thrown whenever the programmer made a mistake
  /// </summary>
  [Serializable]
  [PublicAPI]
  public class ProgrammingErrorException : SystemException
  {
    protected ProgrammingErrorException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }


    [PublicAPI]
    public ProgrammingErrorException()
    {
    }

    /// <inheritdoc />
    /// <summary>
    ///   Constructs a ProgrammingErrorException exception
    /// </summary>
    /// <param name="message">Message associated with the exception</param>
    [PublicAPI]
    public ProgrammingErrorException(string message)
      : base(message)
    {
    }

    [PublicAPI]
    public ProgrammingErrorException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
