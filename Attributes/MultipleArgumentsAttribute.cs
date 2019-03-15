using System;
using JetBrains.Annotations;

namespace NMaier.GetOptNet
{
  /// <inheritdoc />
  /// <summary>
  ///   Specifies for an array/list argument the min/max arguments constraints.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  [PublicAPI]
  public sealed class MultipleArgumentsAttribute : Attribute
  {
    /// <summary>
    ///   Exact number of required parameters
    /// </summary>
    public int Exact
    {
      get => Max;
      set {
        if (value <= 0) {
          throw new ProgrammingErrorException("Exact has to be > 0");
        }

        Min = Max = value;
      }
    }

    /// <summary>
    ///   Maxiumum number of required parameters
    /// </summary>
    public int Max { get; set; }

    /// <summary>
    ///   Minimum number of required parameters
    /// </summary>
    public int Min { get; set; }
  }
}
