using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace NMaier.GetOptNet
{
  /// <inheritdoc />
  /// <summary>
  ///   Defines a flag (boolean) argument
  /// </summary>
  [SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag")]
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  [PublicAPI]
  public sealed class FlagArgumentAttribute : Attribute
  {
    /// <inheritdoc />
    /// <summary>
    ///   Constructor.
    /// </summary>
    /// <param name="whenSet">Value that will be used when set</param>
    public FlagArgumentAttribute(bool whenSet)
    {
      WhenSet = whenSet;
    }


    /// <summary>
    ///   Returns assigned value that will be used when set
    /// </summary>
    /// <returns></returns>
    public bool WhenSet { get; }
  }
}
