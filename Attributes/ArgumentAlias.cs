using System;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Defines an alias name for an argument.
  /// See also: <seealso cref="GetOptOptions.UsageShowAliases"/>
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
  public sealed class ArgumentAlias : Attribute
  {
    private readonly string alias;


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="alias">Name of the alias</param>
    public ArgumentAlias(string alias)
    {
      if (String.IsNullOrEmpty(alias)) {
        throw new ProgrammingError("You must specify a name");
      }
      this.alias = alias;
    }


    /// <summary>
    /// Returns the assigned alias
    /// </summary>
    /// <returns>Alias</returns>
    public string Alias { get { return alias; } }
  }
}
