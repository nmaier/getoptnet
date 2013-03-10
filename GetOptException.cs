using System;
using System.Runtime.Serialization;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Base exception for GetOptNet
  /// </summary>
  [Serializable]
  public class GetOptException : Exception
  {
    protected GetOptException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }


    public GetOptException()
    {
    }
    /// <summary>
    /// Constructs a GetOptException exception
    /// </summary>
    /// <param name="message">Message associated with the exception</param>
    public GetOptException(string message)
      : base(message)
    {
    }
    public GetOptException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
