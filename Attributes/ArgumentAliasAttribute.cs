using System;
using JetBrains.Annotations;
using static System.String;

namespace NMaier.GetOptNet
{
  /// <inheritdoc />
  /// <summary>
  ///   Defines an alias name for an argument.
  ///   See also: <seealso cref="P:NMaier.GetOptNet.GetOptOptionsAttribute.UsageShowAliases" />
  /// </summary>
  [PublicAPI]
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
  public sealed class ArgumentAliasAttribute : Attribute
  {
    /// <inheritdoc />
    /// <summary>
    ///   Constructor.
    /// </summary>
    /// <param name="alias">Name of the alias</param>
    public ArgumentAliasAttribute(string alias)
    {
      if (IsNullOrEmpty(alias)) {
        throw new ProgrammingErrorException("You must specify a name");
      }

      Alias = alias;
    }


    /// <summary>
    ///   Returns the assigned alias
    /// </summary>
    /// <returns>Alias</returns>
    public string Alias { get; }
  }
}
