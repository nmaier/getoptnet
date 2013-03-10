using System;
using System.Runtime.Serialization;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Thrown when a MultipleArgument's min/max contraints aren't fulfilled
  /// </summary>
  [Serializable]
  public class MultipleArgumentCountException : GetOptException
  {
    protected MultipleArgumentCountException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }


    public MultipleArgumentCountException()
    {
    }
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">Exception message</param>
    public MultipleArgumentCountException(string message)
      : base(message)
    {
    }
    public MultipleArgumentCountException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
