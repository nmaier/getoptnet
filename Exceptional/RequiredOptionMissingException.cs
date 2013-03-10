using System;
using System.Globalization;
using System.Runtime.Serialization;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Thrown when a required argument is missing
  /// </summary>
  [Serializable]
  public class RequiredOptionMissingException : GetOptException
  {
    protected RequiredOptionMissingException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }


    internal RequiredOptionMissingException(ArgumentHandler aOption)
      : this(String.Format(CultureInfo.CurrentCulture, "Required option {0} wasn't specified", aOption.Name.ToLower()))
    {
    }


    public RequiredOptionMissingException()
    {
    }
    /// <summary>
    /// Constructs a RequiredOptionMissingException exception
    /// </summary>
    /// <param name="message">Message associated with the exception</param>
    public RequiredOptionMissingException(string message)
      : base(message)
    {
    }
    public RequiredOptionMissingException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
