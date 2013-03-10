using System;
using System.Runtime.Serialization;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Custom exception that is thrown whenever the programmer made a mistake
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), Serializable]
  public class ProgrammingError : SystemException
  {
    protected ProgrammingError(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }


    public ProgrammingError()
    {
    }
    /// <summary>
    /// Constructs a ProgrammingError exception
    /// </summary>
    /// <param name="message">Message associated with the exception</param>
    public ProgrammingError(string message)
      : base(message)
    {
    }
    public ProgrammingError(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
