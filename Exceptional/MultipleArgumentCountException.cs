using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace NMaier.GetOptNet
{
  /// <inheritdoc />
  /// <summary>
  ///   Thrown when a MultipleArgument's min/MaxArguments contraints aren't fulfilled
  /// </summary>
  [Serializable]
  [PublicAPI]
  public class MultipleArgumentCountException : GetOptException
  {
    protected MultipleArgumentCountException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }


    [PublicAPI]
    public MultipleArgumentCountException()
    {
    }

    /// <inheritdoc />
    /// <summary>
    ///   Constructor.
    /// </summary>
    /// <param name="message">Exception message</param>
    [PublicAPI]
    public MultipleArgumentCountException(string message)
      : base(message)
    {
    }

    [PublicAPI]
    public MultipleArgumentCountException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
