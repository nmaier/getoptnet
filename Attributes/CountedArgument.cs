using System;

namespace NMaier.GetOptNet
{
  /// <summary>
  /// Specifies that an argument will not take a value, but instead will count the number of occurances in the user supplied arguments.
  /// </summary>
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix"), AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
  public sealed class CountedArgument : Attribute
  {
  }
}
