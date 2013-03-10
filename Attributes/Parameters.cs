using System;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Specifies the destination of any non-argument strings (aka. parameters) the user supplies.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"),
  AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
  public sealed class Parameters : Attribute
  {
    private string helpVar = "param";


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
    /// Help variable text to be used in usage output
    /// </summary>
    public string HelpVar
    {
      get
      {
        return helpVar;
      }
      set
      {
        helpVar = value;
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
