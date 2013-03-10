using System;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Defines how a duplicate value for an argument is handled
  /// </summary>
  public enum ArgumentCollision
  {
    /// <summary>
    /// Latter value is ignored
    /// </summary>
    Ignore,
    /// <summary>
    /// Latter value should overwrite current
    /// </summary>
    Overwrite,
    /// <summary>
    /// An exception is thrown
    /// </summary>
    Throw
  }
}
