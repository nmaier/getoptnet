using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace NMaier.GetOptNet
{
  /// <inheritdoc />
  /// <summary>
  ///   Base exception for GetOptNet
  /// </summary>
  [Serializable]
  [PublicAPI]
  public class GetOptException : Exception
  {
    protected GetOptException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    [PublicAPI]
    public GetOptException()
    {
    }

    /// <inheritdoc />
    /// <summary>
    ///   Constructs a GetOptException exception
    /// </summary>
    /// <param name="message">Message associated with the exception</param>
    [PublicAPI]
    public GetOptException(string message)
      : base(message)
    {
    }

    [PublicAPI]
    public GetOptException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
