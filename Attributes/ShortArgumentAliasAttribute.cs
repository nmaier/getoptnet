using System;
using JetBrains.Annotations;
using static System.String;

namespace NMaier.GetOptNet
{
  /// <inheritdoc />
  /// <summary>
  ///   Defines an short alias name for an argument.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
  [PublicAPI]
  public sealed class ShortArgumentAliasAttribute : Attribute
  {
    /// <inheritdoc />
    /// <summary>
    ///   Constructor.
    /// </summary>
    /// <param name="alias">Alias</param>
    public ShortArgumentAliasAttribute(char alias)
    {
      if (IsNullOrEmpty(new string(alias, 1).Trim())) {
        throw new ProgrammingErrorException("You must specify a name");
      }

      Alias = alias;
    }


    /// <summary>
    ///   Returns the assigned alias
    /// </summary>
    /// <returns>Alias</returns>
    public char Alias { get; }
  }
}
