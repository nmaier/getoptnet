using System;
using JetBrains.Annotations;
using static System.String;

namespace NMaier.GetOptNet
{
  /// <inheritdoc />
  /// <summary>
  ///   Define an argument.
  /// </summary>
  /// <seealso cref="T:NMaier.GetOptNet.ArgumentAliasAttribute" />
  /// <seealso cref="T:NMaier.GetOptNet.ShortArgumentAttribute" />
  /// <seealso cref="T:NMaier.GetOptNet.ShortArgumentAliasAttribute" />
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  [PublicAPI]
  public sealed class ArgumentAttribute : Attribute
  {
    /// <inheritdoc />
    /// <summary>
    ///   Default constructor. Will use the member's name as argument name
    /// </summary>
    public ArgumentAttribute()
    {
    }

    /// <inheritdoc />
    /// <summary>
    ///   Constructor. Takes a custom name for the argument name.
    /// </summary>
    /// <param name="arg">Custom argument name</param>
    public ArgumentAttribute(string arg)
    {
      if (IsNullOrEmpty(arg)) {
        throw new ProgrammingErrorException("You must specify a name");
      }

      Arg = arg;
    }


    /// <summary>
    ///   User-definable help text.
    ///   See also: <seealso cref="GetOpt.PrintUsage(HelpCategory)" />,
    ///   <seealso cref="GetOpt.AssembleUsage" />
    /// </summary>
    public string HelpText { get; set; } = Empty;

    /// <summary>
    ///   Variable name to display in Usage.
    ///   See also: <seealso cref="GetOpt.PrintUsage(HelpCategory)" />,
    ///   <seealso cref="GetOpt.AssembleUsage" />
    /// </summary>
    public string HelpVar { get; set; } = Empty;

    /// <summary>
    ///   Defines how to handle arguments the user supplied more than once.
    ///   Has no meaning for "flag" and array/list attributes.
    /// </summary>
    public ArgumentCollision OnCollision { get; set; } = ArgumentCollision.Ignore;

    /// <summary>
    ///   Defines if the argument in question is required and hence cannot be omitted
    /// </summary>
    public bool Required { get; set; }


    /// <summary>
    ///   Returns the name of the argument. Might be empty, indicating the member's name should be used
    /// </summary>
    /// <returns>Name of the argument</returns>
    public string Arg { get; } = Empty;

    /// <summary>
    ///   Help category for usage generation
    /// </summary>
    public HelpCategory Category { get; set; } = HelpCategory.Basic;
  }
}
