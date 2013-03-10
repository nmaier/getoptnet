using System;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Defines a short argument.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
  public sealed class ShortArgument : Attribute
  {
    private readonly char arg;


    /// <summary>
    /// Constructor. Supply the short argument name.
    /// </summary>
    /// <param name="arg">Argument name</param>
    public ShortArgument(char arg)
    {
      if (String.IsNullOrEmpty(new string(arg, 1).Trim())) {
        throw new ProgrammingError("You must specify a name");
      }
      this.arg = arg;
    }


    /// <summary>
    /// Returns the assigned argument name. Is always set.
    /// </summary>
    /// <returns>Argument name</returns>
    public char Arg { get { return arg; } }
  }
}
