using System;
using JetBrains.Annotations;
using static System.String;

namespace NMaier.GetOptNet
{
  /// <inheritdoc />
  /// <summary>
  ///   Defines a short argument.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  [PublicAPI]
  public sealed class ShortArgumentAttribute : Attribute
  {
    /// <inheritdoc />
    /// <summary>
    ///   Constructor. Supply the short argument name.
    /// </summary>
    /// <param name="arg">ArgumentAttribute name</param>
    public ShortArgumentAttribute(char arg)
    {
      if (IsNullOrEmpty(new string(arg, 1).Trim())) {
        throw new ProgrammingErrorException("You must specify a name");
      }

      Arg = arg;
    }


    /// <summary>
    ///   Returns the assigned argument name. Is always set.
    /// </summary>
    /// <returns>ArgumentAttribute name</returns>
    public char Arg { get; }
  }
}
