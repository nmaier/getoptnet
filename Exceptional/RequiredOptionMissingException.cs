using System;
using System.Globalization;
using System.Runtime.Serialization;
using JetBrains.Annotations;
using static System.String;

namespace NMaier.GetOptNet
{
  /// <inheritdoc />
  /// <summary>
  ///   Thrown when a required argument is missing
  /// </summary>
  [Serializable]
  [PublicAPI]
  public class RequiredOptionMissingException : GetOptException
  {
    protected RequiredOptionMissingException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }


    [PublicAPI]
    internal RequiredOptionMissingException(ArgumentHandler aOption)
      : this(Format(CultureInfo.CurrentCulture, "Required option {0} wasn't specified", aOption.Name.ToLower()))
    {
    }


    [PublicAPI]
    public RequiredOptionMissingException()
    {
    }

    /// <inheritdoc />
    /// <summary>
    ///   Constructs a RequiredOptionMissingException exception
    /// </summary>
    /// <param name="message">Message associated with the exception</param>
    [PublicAPI]
    public RequiredOptionMissingException(string message)
      : base(message)
    {
    }

    [PublicAPI]
    public RequiredOptionMissingException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
