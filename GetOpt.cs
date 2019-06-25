using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

[assembly: CLSCompliant(true)]

namespace NMaier.GetOptNet
{
  /// <summary>
  ///   Simple command line processing. Takes commandline arguments, processes them and assign them to the appropriate
  ///   fields or values in your derived class.
  ///   <seealso cref="GetOptOptionsAttribute" />
  /// </summary>
  [GetOptOptions]
  [PublicAPI]
  public abstract partial class GetOpt
  {
    private static readonly Regex regDashesDie = new Regex(
      "^\\s*--\\s*$", RegexOptions.Compiled);

    private static readonly Regex regDashesLong = new Regex(
      "^\\s*--([\\w\\d_-]+)(?:=(.+)|\\s*)?$", RegexOptions.Compiled);

    private static readonly Regex regDashesShort = new Regex(
      "^\\s*-([\\w\\d].*?)\\s*$", RegexOptions.Compiled);

    private static readonly Regex regSlashes = new Regex(
      "^\\s*/([\\w\\d_-]+)(?:=(.+)|\\s*)?$", RegexOptions.Compiled);

    private readonly List<ArgumentHandler> handlers = new List<ArgumentHandler>();

    private readonly Dictionary<string, ArgumentHandler> longs =
      new Dictionary<string, ArgumentHandler>();

    private readonly Dictionary<char, ArgumentHandler> shorts =
      new Dictionary<char, ArgumentHandler>();

    private GetOptOptionsAttribute opts;

    private MultipleArgumentHandler parameters;

    /// <summary>
    ///   Constructs a new command line parser instance.
    /// </summary>
    protected GetOpt()
    {
      Initialize();
    }

    /// <summary>
    ///   Ends the parsing of commandline arguments and assigns the parsed values to the corresponding members
    ///   <seealso cref="GetOpt.Update(IEnumerable{string})" />
    /// </summary>
    /// <exception cref="ProgrammingErrorException">You messed something up</exception>
    /// <exception cref="UnknownAttributeException">
    ///   The user supplied an argument that isn't recognized from it's name.
    ///   <see cref="GetOptOptionsAttribute.OnUnknownArgument" />
    /// </exception>
    /// <exception cref="InvalidValueException">
    ///   The user supplied a value for an argument that cannot parsed to the correct
    ///   type.
    /// </exception>
    /// <exception cref="DuplicateArgumentException">
    ///   The user supplied an argument more than once and the argument type does
    ///   not allow this. <see cref="ArgumentAttribute.OnCollision" />
    /// </exception>
    [PublicAPI]
    public void Parse()
    {
      Parse(new string[0]);
    }

    /// <summary>
    ///   Ends the parsing of commandline arguments and assigns the parsed values to the corresponding members
    ///   <seealso cref="GetOpt.Update(IEnumerable{string})" />
    /// </summary>
    /// <exception cref="ProgrammingErrorException">You messed something up</exception>
    /// <exception cref="UnknownAttributeException">
    ///   The user supplied an argument that isn't recognized from it's name.
    ///   <see cref="GetOptOptionsAttribute.OnUnknownArgument" />
    /// </exception>
    /// <exception cref="InvalidValueException">
    ///   The user supplied a value for an argument that cannot parsed to the correct
    ///   type.
    /// </exception>
    /// <exception cref="DuplicateArgumentException">
    ///   The user supplied an argument more than once and the argument type does
    ///   not allow this. <see cref="ArgumentAttribute.OnCollision" />
    /// </exception>
    /// <param name="args">Arguments to parse</param>
    public void Parse(IEnumerable<string> args)
    {
      Update(args);
      foreach (var h in handlers) {
        try {
          h.Finish();
        }
        catch (NotSupportedException) {
          throw new InvalidValueException("Argument has a different type");
        }
      }
    }
  }
}
