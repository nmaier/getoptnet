namespace NMaier.GetOptNet
{
  /// <summary>
  ///   Specifies what to do when an unknown (unspecified) argument is encountered.
  /// </summary>
  public enum UnknownArgumentsAction
  {
    /// <summary>
    ///   ArgumentAttribute should be ignored
    /// </summary>
    Ignore,

    /// <summary>
    ///   Place both argument name and value (if any) in ParametersAttribute
    /// </summary>
    PlaceInParameters,

    /// <summary>
    ///   An exception should be thrown. That is an UnknownArgumentException
    /// </summary>
    Throw
  }
}
