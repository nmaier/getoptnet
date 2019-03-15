using System;

namespace NMaier.GetOptNet
{
  /// <summary>
  ///   Specifies which type of arguments to support. Either unix-esque using dashes, windows-esque using slashes or both.
  ///   None specifies that no arguments at all will be accepted. Instead only parameters are populated.
  /// </summary>
  [Flags]
  public enum ArgumentPrefixTypes
  {
    /// <summary>
    ///   Use both types intermixed.
    /// </summary>
    Both = 3,

    /// <summary>
    ///   Use unix-esque "dashed" arguments
    /// </summary>
    Dashes = 1,

    /// <summary>
    ///   No arguments at all.
    /// </summary>
    None = 0,

    /// <summary>
    ///   Use windows-esque "slashed" arguments
    /// </summary>
    Slashes = 2
  }
}
