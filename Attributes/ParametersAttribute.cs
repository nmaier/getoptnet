using System;
using JetBrains.Annotations;

namespace NMaier.GetOptNet
{
  /// <inheritdoc />
  /// <summary>
  ///   Specifies the destination of any non-argument strings (aka. parameters) the user supplies.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
  [PublicAPI]
  public sealed class ParametersAttribute : Attribute
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
    ///   Help variable text to be used in usage output
    /// </summary>
    public string HelpVar { get; set; } = string.Empty;

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
