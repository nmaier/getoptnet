using System;

namespace NMaier.GetOptNet
{

  /// <summary>
  /// Defines an short alias name for an argument.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
  public sealed class ShortArgumentAlias : Attribute
  {
    private readonly char alias;


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="alias">Alias</param>
    public ShortArgumentAlias(char alias)
    {
      if (String.IsNullOrEmpty(new string(alias, 1).Trim())) {
        throw new ProgrammingError("You must specify a name");
      }
      this.alias = alias;
    }


    /// <summary>
    /// Returns the assigned alias
    /// </summary>
    /// <returns>Alias</returns>
    public char Alias { get { return alias; } }
  }
}
