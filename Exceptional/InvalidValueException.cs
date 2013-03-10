using System;
using System.Runtime.Serialization;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Thrown when the user supplied a value for an argument that isn't compatible with the argument type
  /// </summary>
  [Serializable]
  public class InvalidValueException : GetOptException
  {
    protected InvalidValueException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }


    public InvalidValueException()
    {
    }
    /// <summary>
    /// Constructs a InvalidValueException exception
    /// </summary>
    /// <param name="message">Message associated with the exception</param>
    public InvalidValueException(string message)
      : base(message)
    {
    }
    public InvalidValueException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
