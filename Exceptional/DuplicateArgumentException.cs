using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace NMaier.GetOptNet
{
  /// <inheritdoc />
  /// <summary>
  ///   Thrown when the user supplied an argument more than once and the argument does not support this.
  ///   <seealso cref="P:NMaier.GetOptNet.ArgumentAttribute.OnCollision" />
  /// </summary>
  [Serializable]
  [PublicAPI]
  public class DuplicateArgumentException : GetOptException
  {
    protected DuplicateArgumentException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }


    [PublicAPI]
    public DuplicateArgumentException()
    {
    }

    /// <inheritdoc />
    /// <summary>
    ///   Constructs a DuplicateArgumentException exception
    /// </summary>
    /// <param name="message">Message associated with the exception</param>
    [PublicAPI]
    public DuplicateArgumentException(string message)
      : base(message)
    {
    }

    [PublicAPI]
    public DuplicateArgumentException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
