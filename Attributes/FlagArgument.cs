using System;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Defines a flag (boolean) argument
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flag"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
  public sealed class FlagArgument : Attribute
  {
    private readonly bool whenSet = false;


    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="whenSet">Value that will be used when set</param>
    public FlagArgument(bool whenSet)
    {
      this.whenSet = whenSet;
    }


    /// <summary>
    /// Returns assigned value that will be used when set
    /// </summary>
    /// <returns></returns>
    public bool WhenSet { get { return whenSet; }}
  }
}
