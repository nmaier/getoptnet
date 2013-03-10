using System;
using System.Runtime.Serialization;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Thrown when an unknown attribute is supplied by the user.
  /// <seealso cref="GetOptOptions.OnUnknownArgument"/>
  /// </summary>
  [Serializable]
  public class UnknownAttributeException : GetOptException
  {
    protected UnknownAttributeException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }


    public UnknownAttributeException()
    {
    }
    /// <summary>
    /// Constructs a UnknownAttributeException exception
    /// </summary>
    /// <param name="message">Message associated with the exception</param>
    public UnknownAttributeException(string message)
      : base(message)
    {
    }
    public UnknownAttributeException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
