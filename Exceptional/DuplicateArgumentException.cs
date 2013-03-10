using System;
using System.Runtime.Serialization;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Thrown when the user supplied an argument more than once and the argument does not support this.
  /// <seealso cref="Argument.OnCollision"/>
  /// </summary>
  [Serializable]
  public class DuplicateArgumentException : GetOptException
  {
    protected DuplicateArgumentException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }


    public DuplicateArgumentException()
    {
    }
    /// <summary>
    /// Constructs a DuplicateArgumentException exception
    /// </summary>
    /// <param name="message">Message associated with the exception</param>
    public DuplicateArgumentException(string message)
      : base(message)
    {
    }
    public DuplicateArgumentException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
