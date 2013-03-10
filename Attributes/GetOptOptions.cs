using System;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Defines Options for GetOpt derived classes
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public sealed class GetOptOptions : Attribute
  {
    private ArgumentPrefixTypes apt = ArgumentPrefixTypes.Both;

    private ArgumentCaseType act = ArgumentCaseType.Insensitive;

    private ArgumentPrefixTypes upt = ArgumentPrefixTypes.Dashes;

    private UnknownArgumentsAction uaa = UnknownArgumentsAction.Throw;

    private UsageAliasShowOption aso = UsageAliasShowOption.Omit;

    private string usageIntro = string.Empty;

    private string usageEpilog = string.Empty;

    /// <summary>
    /// Defines which argument type to accept
    /// </summary>
    public ArgumentPrefixTypes AcceptPrefixType
    {
      get {
        return apt;
      }
      set {
        apt = value;
      }
    }
    /// <summary>
    /// Defines the character case the user may specify arguments in
    /// </summary>
    public ArgumentCaseType CaseType
    {
      get {
        return act;
      }
      set {
        act = value;
      }
    }
    /// <summary>
    /// Defines how to handle unknown arguments
    /// </summary>
    public UnknownArgumentsAction OnUnknownArgument
    {
      get {
        return uaa;
      }
      set {
        uaa = value;
      }
    }
    /// <summary>
    /// Defines the epilog text of usage
    /// </summary>
    public string UsageEpilog
    {
      get {
        return usageEpilog;
      }
      set {
        usageEpilog = value;
      }
    }
    /// <summary>
    /// Defines the introductory text of Usage
    /// </summary>
    public string UsageIntro
    {
      get {
        return usageIntro;
      }
      set {
        usageIntro = value;
      }
    }
    /// <summary>
    /// Defines what argument type to use in Usage. Cannot be None or Both.
    /// </summary>
    public ArgumentPrefixTypes UsagePrefix
    {
      get {
        return upt;
      }
      set {
        if (value != ArgumentPrefixTypes.Dashes && value != ArgumentPrefixTypes.Slashes) {
          throw new ProgrammingError("Usage prefix must be Dashes or Slashes");
        }
        upt = value;
      }
    }
    /// <summary>
    /// Defines wether to show or omit aliases in Usage
    /// </summary>
    public UsageAliasShowOption UsageShowAliases
    {
      get {
        return aso;
      }
      set {
        aso = value;
      }
    }
  }
}
