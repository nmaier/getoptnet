using System;
using JetBrains.Annotations;

namespace NMaier.GetOptNet
{
  /// <inheritdoc />
  /// <summary>
  ///   Defines Options for GetOpt derived classes
  /// </summary>
  [AttributeUsage(AttributeTargets.Class)]
  [PublicAPI]
  public sealed class GetOptOptionsAttribute : Attribute
  {
    private ArgumentPrefixTypes usagePrefix = ArgumentPrefixTypes.Dashes;

    /// <summary>
    ///   Defines which argument type to accept
    /// </summary>
    public ArgumentPrefixTypes AcceptPrefixType { get; set; } = ArgumentPrefixTypes.Both;

    /// <summary>
    ///   Defines the character case the user may specify arguments in
    /// </summary>
    public ArgumentCaseType CaseType { get; set; } = ArgumentCaseType.Insensitive;

    /// <summary>
    ///   Defines how to handle unknown arguments
    /// </summary>
    public UnknownArgumentsAction OnUnknownArgument { get; set; } = UnknownArgumentsAction.Throw;

    /// <summary>
    ///   Defines the epilog text of usage
    /// </summary>
    public string UsageEpilog { get; set; } = string.Empty;

    /// <summary>
    ///   Defines the introductory text of Usage
    /// </summary>
    public string UsageIntro { get; set; } = string.Empty;

    /// <summary>
    ///   Defines what argument type to use in Usage. Cannot be None or Both.
    /// </summary>
    public ArgumentPrefixTypes UsagePrefix
    {
      get => usagePrefix;
      set {
        if (value != ArgumentPrefixTypes.Dashes &&
            value != ArgumentPrefixTypes.Slashes) {
          throw new ProgrammingErrorException("Usage prefix must be Dashes or Slashes");
        }

        usagePrefix = value;
      }
    }

    /// <summary>
    ///   Defines wether to show or omit aliases in Usage
    /// </summary>
    public UsageAliasShowOption UsageShowAliases { get; set; } = UsageAliasShowOption.Omit;
  }
}
