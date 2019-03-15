using JetBrains.Annotations;

namespace NMaier.GetOptNet
{
  /// <summary>
  ///   Defines the case the user may specify arguments in
  /// </summary>
  [PublicAPI]
  public enum ArgumentCaseType
  {
    /// <summary>
    ///   Use argument name character case as defined in the program (aka. case sensitive)
    /// </summary>
    AsDefined,

    /// <summary>
    ///   User may specify arguments using any case.
    /// </summary>
    Insensitive,

    /// <summary>
    ///   Use only lower case argument names
    /// </summary>
    OnlyLower
  }
}
