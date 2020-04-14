using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace NMaier.GetOptNet
{
  /// <inheritdoc />
  /// <summary>
  ///   Thrown when the user supplied a value for an argument that isn't compatible with the argument type
  /// </summary>
  [Serializable]
  [PublicAPI]
  public sealed class InvalidValueException : GetOptException
  {
    private InvalidValueException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }


    [PublicAPI]
    public InvalidValueException()
    {
    }

    /// <inheritdoc />
    /// <summary>
    ///   Constructs a InvalidValueException exception
    /// </summary>
    /// <param name="message">Message associated with the exception</param>
    [PublicAPI]
    public InvalidValueException(string message)
      : base(message)
    {
    }

    [PublicAPI]
    public InvalidValueException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
