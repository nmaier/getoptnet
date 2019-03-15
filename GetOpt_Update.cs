using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using static System.String;

namespace NMaier.GetOptNet
{
  public abstract partial class GetOpt
  {
    private static void UpdateHandler(ArgumentHandler handler, string value, string arg)
    {
      try {
        handler.Assign(value);
      }
      catch (ArgumentException ex) {
        throw new ProgrammingErrorException(ex.Message);
      }
      catch (NotSupportedException ex) {
        throw new InvalidValueException(
          Format(
            CultureInfo.CurrentCulture,
            "Wrong value type for argument \"{0}\": {1}", arg, ex.Message));
      }
      catch (GetOptException) {
        throw;
      }
      catch (TargetInvocationException ex) {
        switch (ex.InnerException) {
        case GetOptException _:
          throw ex.InnerException;
        case NotSupportedException _:
          throw new InvalidValueException(
            Format(
              CultureInfo.CurrentCulture,
              "Wrong value type for argument \"{0}\": {1}", arg, ex.Message));
        default:
          throw new ProgrammingErrorException(ex.Message, ex);
        }
      }
      catch (Exception ex) {
        throw new ProgrammingErrorException(ex.Message, ex);
      }
    }

    /// <summary>
    ///   Updates the parsed argument collection, but does not yet assign it back.
    ///   See Also: <seealso cref="GetOpt.Parse()" />
    /// </summary>
    /// <exception cref="ProgrammingErrorException">You messed something up</exception>
    /// <exception cref="UnknownAttributeException">
    ///   The user supplied an argument that isn't recognized from it's name.
    ///   <see cref="GetOptOptionsAttribute.OnUnknownArgument" />
    /// </exception>
    /// <exception cref="InvalidValueException">
    ///   The user supplied a value for an argument that cannot parsed to the correct
    ///   type.
    /// </exception>
    /// <exception cref="DuplicateArgumentException">
    ///   The user supplied an argument more than once and the argument type does
    ///   not allow this. <see cref="ArgumentAttribute.OnCollision" />
    /// </exception>
    /// <param name="args">Arguments to parse</param>
    public void Update(IEnumerable<string> args)
    {
      if (args == null) {
        throw new ArgumentException("args may not be null");
      }

      var enumerator = args.GetEnumerator();
      while (enumerator.MoveNext()) {
        var current = enumerator.Current;
        if ((opts.AcceptPrefixType & ArgumentPrefixTypes.Dashes) != 0) {
          var result = MaybeHandleDashArgument(current, enumerator);
          if (result == HandleResult.Handled) {
            continue;
          }

          if (result == HandleResult.Stop) {
            break;
          }
        }

        if ((opts.AcceptPrefixType & ArgumentPrefixTypes.Slashes) != 0) {
          var result = MaybeHandleSlashArgument(current, enumerator);
          if (result == HandleResult.Handled) {
            continue;
          }

          if (result == HandleResult.Stop) {
            break;
          }
        }

        UpdateParameters(current);
      }

      // Consume remainder
      while (enumerator.MoveNext()) {
        UpdateParameters(enumerator.Current);
      }
    }

    private void HandleUnknownArgument(string arg, string val)
    {
      switch (opts.OnUnknownArgument) {
      case UnknownArgumentsAction.Throw:
        throw new UnknownAttributeException(
          $"There is no argument with the name \"{arg}\"");

      case UnknownArgumentsAction.PlaceInParameters:
        UpdateParameters(arg);
        if (!IsNullOrEmpty(val)) {
          UpdateParameters(val);
        }

        break;
      case UnknownArgumentsAction.Ignore:
        break;
      default:
        throw new ArgumentOutOfRangeException();
      }
    }

    private HandleResult MaybeHandleDashArgument(string c, IEnumerator<string> e)
    {
      if (regDashesDie.IsMatch(c)) {
        return HandleResult.Stop;
      }

      var m = regDashesLong.Match(c);
      if (m.Success) {
        var longArg = m.Groups[1].Value;
        if (opts.CaseType == ArgumentCaseType.Insensitive) {
          longArg = longArg.ToLower();
        }

        var val = m.Groups[2].Value;
        if (!longs.TryGetValue(longArg, out var h)) {
          HandleUnknownArgument(longArg, val);
          return HandleResult.Handled;
        }

        if (!IsNullOrEmpty(val) && h.IsFlag) {
          throw new InvalidValueException(
            Format(
              CultureInfo.CurrentCulture,
              "Argument \"{0}\" does not except a value", longArg));
        }

        if (IsNullOrEmpty(val) && !h.IsFlag) {
          throw new InvalidValueException(
            Format(
              CultureInfo.CurrentCulture, "Omitted value for argument \"{0}\"", longArg));
        }

        UpdateHandler(h, val, longArg);
        return HandleResult.Handled;
      }

      m = regDashesShort.Match(c);
      if (!m.Success) {
        return HandleResult.NotHandled;
      }

      var arg = m.Groups[1].Value;
      var singles = arg.ToCharArray();
      for (var i = 0; i < singles.Length; ++i) {
        var currentArg = singles[i];
        if (!shorts.TryGetValue(currentArg, out var h)) {
          HandleUnknownArgument(new string(currentArg, 1), null);
          continue;
        }

        if (!h.IsFlag) {
          var val = i != singles.Length - 1 ? arg.Substring(++i) : null;
          if (IsNullOrEmpty(val) && e.MoveNext()) {
            val = e.Current;
          }

          if (IsNullOrEmpty(val)) {
            throw new InvalidValueException($"Omitted value for argument \"{currentArg}\"");
          }

          UpdateHandler(h, val, new string(currentArg, 1));
          break;
        }

        UpdateHandler(h, null, new string(currentArg, 1));
      }

      return HandleResult.Handled;
    }

    private HandleResult MaybeHandleSlashArgument(string c, IEnumerator<string> e)
    {
      var match = regSlashes.Match(c);
      if (!match.Success) {
        return HandleResult.NotHandled;
      }

      var arg = match.Groups[1].Value;
      var val = match.Groups[2].Value;
      ArgumentHandler handler;
      if (arg.Length == 1) {
        var shortarg = arg[0];
        if (!shorts.TryGetValue(shortarg, out handler)) {
          HandleUnknownArgument(arg, val);
          return HandleResult.Handled;
        }

        if (!IsNullOrEmpty(val) && handler.IsFlag) {
          throw new InvalidValueException(
            Format(
              CultureInfo.CurrentCulture,
              "Argument \"{0}\" does not except a value", arg));
        }

        if (IsNullOrEmpty(val) && !handler.IsFlag) {
          val = e.MoveNext() ? e.Current : val;
          if (IsNullOrEmpty(val)) {
            throw new InvalidValueException(
              Format(
                CultureInfo.CurrentCulture,
                "Omitted value for argument \"{0}\"", arg));
          }
        }

        UpdateHandler(handler, val, arg);
        return HandleResult.Handled;
      }

      if (opts.CaseType == ArgumentCaseType.Insensitive) {
        arg = arg.ToLower();
      }

      if (!longs.TryGetValue(arg, out handler)) {
        HandleUnknownArgument(arg, val);
        return HandleResult.Handled;
      }

      if (IsNullOrEmpty(val) && !handler.IsFlag) {
        throw new InvalidValueException(
          Format(
            CultureInfo.CurrentCulture,
            "Argument \"{0}\" does except a value", arg));
      }

      UpdateHandler(handler, val, arg);
      return HandleResult.Handled;
    }

    private void UpdateParameters(string value)
    {
      if (parameters == null) {
        if (opts.OnUnknownArgument == UnknownArgumentsAction.Ignore) {
          return;
        }

        throw new UnknownAttributeException(
          "This program does not support positional arguments");
      }

      UpdateHandler(parameters, value, "<parameters>");
    }

    private enum HandleResult
    {
      Handled,
      NotHandled,
      Stop
    }
  }
}
