using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace NMaier.GetOptNet
{
  /// <inheritdoc />
  /// <summary>
  ///   Thrown when an unknown attribute is supplied by the user.
  ///   <seealso cref="P:NMaier.GetOptNet.GetOptOptionsAttribute.OnUnknownArgument" />
  /// </summary>
  [Serializable]
  [PublicAPI]
  public class UnknownAttributeException : GetOptException
  {
    protected UnknownAttributeException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }


    [PublicAPI]
    public UnknownAttributeException()
    {
    }

    /// <inheritdoc />
    /// <summary>
    ///   Constructs a UnknownAttributeException exception
    /// </summary>
    /// <param name="message">Message associated with the exception</param>
    [PublicAPI]
    public UnknownAttributeException(string message)
      : base(message)
    {
    }

    [PublicAPI]
    public UnknownAttributeException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
