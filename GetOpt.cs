using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

[assembly:CLSCompliant(true)]
namespace NMaier.GetOptNet
{
  /// <summary>
  /// Simple command line processing. Takes commandline arguments, processes them and assign them to the appropriate
  /// fields or values in your derived class.
  /// <seealso cref="GetOptOptions"/>
  /// </summary>
  [GetOptOptions]
  public abstract partial class GetOpt
  {
    private GetOptOptions opts;

    private MultipleArgumentHandler parameters;

    private readonly static Regex regDashesLong = new Regex("^\\s*--([\\w\\d_-]+)(?:=(.+)|\\s*)?$", RegexOptions.Compiled);

    private readonly static Regex regDashesShort = new Regex("^\\s*-([\\w\\d]+)\\s*$", RegexOptions.Compiled);

    private readonly static Regex regDashesDie = new Regex("^\\s*--\\s*$", RegexOptions.Compiled);

    private readonly static Regex regSlashes = new Regex("^\\s*/([\\w\\d_-]+)(?:=(.+)|\\s*)?$", RegexOptions.Compiled);

    private readonly Dictionary<string, ArgumentHandler> longs = new Dictionary<string, ArgumentHandler>();

    private readonly Dictionary<char, ArgumentHandler> shorts = new Dictionary<char, ArgumentHandler>();

    private readonly List<ArgumentHandler> handlers = new List<ArgumentHandler>();


    /// <summary>
    /// Constructs a new command line parser instance.
    /// </summary>
    protected GetOpt()
    {
      Initialize();
    }


    /// <summary>
    /// Ends the parsing of commandline arguments and assigns the parsed values to the corresponding members
    /// <seealso cref="GetOpt.Update(string[])"/>
    /// </summary>
    /// <exception cref="ProgrammingError">You messed something up</exception>
    /// <exception cref="UnknownAttributeException">The user supplied an argument that isn't recognized from it's name. <see cref="GetOptOptions.OnUnknownArgument"/></exception>
    /// <exception cref="InvalidValueException">The user supplied a value for an argument that cannot parsed to the correct type.</exception>
    /// <exception cref="DuplicateArgumentException">The user supplied an argument more than once and the argument type does not allow this. <see cref="Argument.OnCollision"/></exception>
    public void Parse()
    {
      Parse(new List<string>());
    }

    /// <summary>
    /// Ends the parsing of commandline arguments and assigns the parsed values to the corresponding members, taking a last array of arguments.
    /// <seealso cref="GetOpt.Update(string[])"/>
    /// </summary>
    /// <exception cref="ProgrammingError">You messed something up</exception>
    /// <exception cref="UnknownAttributeException">The user supplied an argument that isn't recognized from it's name. <see cref="GetOptOptions.OnUnknownArgument"/></exception>
    /// <exception cref="InvalidValueException">The user supplied a value for an argument that cannot parsed to the correct type.</exception>
    /// <exception cref="DuplicateArgumentException">The user supplied an argument more than once and the argument type does not allow this. <see cref="Argument.OnCollision"/></exception>
    /// <param name="args">Additional arguments</param>
    public void Parse(string[] args)
    {
      Parse(new List<string>(args));
    }

    /// <summary>
    /// Ends the parsing of commandline arguments and assigns the parsed values to the corresponding members
    /// <seealso cref="GetOpt.Update(string[])"/>
    /// </summary>
    /// <exception cref="ProgrammingError">You messed something up</exception>
    /// <exception cref="UnknownAttributeException">The user supplied an argument that isn't recognized from it's name. <see cref="GetOptOptions.OnUnknownArgument"/></exception>
    /// <exception cref="InvalidValueException">The user supplied a value for an argument that cannot parsed to the correct type.</exception>
    /// <exception cref="DuplicateArgumentException">The user supplied an argument more than once and the argument type does not allow this. <see cref="Argument.OnCollision"/></exception>
    /// <param name="args">Arguments to parse</param>
    public void Parse(IList<string> args)
    {
      Update(args);
      foreach (ArgumentHandler h in handlers) {
        try {
          h.Finish();
        }
        catch (NotSupportedException) {
          throw new InvalidValueException("Argument has a different type");
        }
      }
    }

    /// <summary>
    /// Updates the parsed argument collection, but does not yet assign it back.
    /// <seealso cref="GetOpt.Parse()"/>
    /// </summary>
    /// <exception cref="ProgrammingError">You messed something up</exception>
    /// <exception cref="UnknownAttributeException">The user supplied an argument that isn't recognized from it's name. <see cref="GetOptOptions.OnUnknownArgument"/></exception>
    /// <exception cref="InvalidValueException">The user supplied a value for an argument that cannot parsed to the correct type.</exception>
    /// <exception cref="DuplicateArgumentException">The user supplied an argument more than once and the argument type does not allow this. <see cref="Argument.OnCollision"/></exception>
    /// <param name="args">Arguments to parse</param>
    public void Update(string[] args)
    {
      Update(new List<string>(args));
    }
  }
}
