using System;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Specifies for an array/list argument the min/max constraints.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
  public sealed class MultipleArguments : Attribute
  {
    /// <summary>
    /// Exact number of required parameters
    /// </summary>
    public int Exact
    {
      get
      {
        return Max;
      }
      set
      {
        if (value <= 0) {
          throw new ProgrammingError("Exact has to be > 0");
        }
        Min = Max = value;
      }
    }
    /// <summary>
    /// Maxiumum number of required parameters
    /// </summary>
    public int Max { get; set; }

    /// <summary>
    /// Minimum number of required parameters
    /// </summary>
    public int Min { get; set; }
  }
}
