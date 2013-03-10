using System;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Define an argument.
  /// </summary>
  /// <seealso cref="ArgumentAlias"/>
  /// <seealso cref="ShortArgument"/>
  /// <seealso cref="ShortArgumentAlias"/>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
  public sealed class Argument : Attribute
  {
    private readonly string arg = string.Empty;

    private string helpText = string.Empty;

    private string helpVar = string.Empty;

    private ArgumentCollision collision = ArgumentCollision.Ignore;

    private bool required = false;


    /// <summary>
    /// Default constructor. Will use the member's name as argument name
    /// </summary>
    public Argument()
    {
    }
    /// <summary>
    /// Constructor. Takes a custom name for the argument name.
    /// </summary>
    /// <param name="arg">Custom argument name</param>
    public Argument(string arg)
    {
      if (String.IsNullOrEmpty(arg)) {
        throw new ProgrammingError("You must specify a name");
      }
      this.arg = arg;
    }


    /// <summary>
    /// User-definable help text.
    /// See also: <seealso cref="GetOpt.PrintUsage()"/>, <seealso cref="GetOpt.AssembleUsage(int)"/>
    /// </summary>
    public string HelpText
    {
      get
      {
        return helpText;
      }
      set
      {
        helpText = value;
      }
    }
    /// <summary>
    /// Variable name to display in Usage.
    /// See also: <seealso cref="GetOpt.PrintUsage()"/>, <seealso cref="GetOpt.AssembleUsage(int)"/>
    /// </summary>
    public string HelpVar
    {
      get
      {
        return helpVar;
      }
      set
      {
        helpVar = value;
      }
    }
    /// <summary>
    /// Defines how to handle arguments the user supplied more than once.
    /// Has no meaning for "flag" and array/list attributes. 
    /// </summary>
    public ArgumentCollision OnCollision
    {
      get
      {
        return collision;
      }
      set
      {
        collision = value;
      }
    }
    /// <summary>
    /// Defines if the argument in question is required and hence cannot be omitted
    /// </summary>
    public bool Required
    {
      get
      {
        return required;
      }
      set
      {
        required = value;
      }
    }


    /// <summary>
    /// Returns the name of the argument. Might be empty, indicating the member's name should be used
    /// </summary>
    /// <returns>Name of the argument</returns>
    public string Arg { get { return arg; } }
  }
}
