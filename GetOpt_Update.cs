using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace NMaier.GetOptNet
{
  public abstract partial class GetOpt
  {
    private enum HandleResult
    {
      Handled,
      NotHandled,
      Stop
    }


    private void HandleUnknownArgument(string arg, string val)
    {
      switch (opts.OnUnknownArgument) {
        case UnknownArgumentsAction.Throw:
          throw new UnknownAttributeException(String.Format("There is no argument with the name \"{0}\"", arg));

        case UnknownArgumentsAction.PlaceInParameters:
          parameters.Assign(arg);
          if (!String.IsNullOrEmpty(val)) {
            parameters.Assign(val);
          }
          break;
      }
    }

    private HandleResult MaybeHandleDashArgument(string c, IEnumerator<string> e)
    {
      if (regDashesDie.IsMatch(c)) {
        return HandleResult.Stop;
      }
      var m = regDashesLong.Match(c);
      if (m.Success) {
        var arg = m.Groups[1].Value;
        if (opts.CaseType == ArgumentCaseType.Insensitive) {
          arg = arg.ToLower();
        }

        var val = m.Groups[2].Value;
        if (!longs.ContainsKey(arg)) {
          HandleUnknownArgument(arg, val);
          return HandleResult.Handled;
        }
        var h = longs[arg];
        if (!String.IsNullOrEmpty(val) && h.IsFlag) {
          throw new InvalidValueException(String.Format(CultureInfo.CurrentCulture, "Argument \"{0}\" does not except a value", arg));
        }
        else if (String.IsNullOrEmpty(val) && !h.IsFlag) {
          throw new InvalidValueException(String.Format(CultureInfo.CurrentCulture, "Omitted value for argument \"{0}\"", arg));
        }
        UpdateHandler(h, val, arg);
        return HandleResult.Handled;
      }

      m = regDashesShort.Match(c);
      if (m.Success) {
        var arg = m.Groups[1].Value;
        var singles = arg.ToCharArray();
        for (var i = 0; i < singles.Length; ++i) {
          var currentArg = singles[i];
          if (!shorts.ContainsKey(currentArg)) {
            HandleUnknownArgument(new string(currentArg, 1), null);
            continue;
          }
          var h = shorts[currentArg];
          if (!h.IsFlag) {
            string val;
            if (i != singles.Length - 1) {
              val = arg.Substring(++i);
            }
            else {
              val = null;
            }
            if (String.IsNullOrEmpty(val) && e.MoveNext()) {
              val = e.Current;
            }

            if (String.IsNullOrEmpty(val)) {
              throw new InvalidValueException(String.Format("Omitted value for argument \"{0}\"", currentArg));
            }
            UpdateHandler(h, val, new string(currentArg, 1));
            break;
          }
          UpdateHandler(h, null, new string(currentArg, 1));
          continue;
        }
        return HandleResult.Handled;
      }

      return HandleResult.NotHandled;
    }

    private HandleResult MaybeHandleSlashArgument(string c, IEnumerator<string> e)
    {
      var m = regSlashes.Match(c);
      if (m.Success) {
        var arg = m.Groups[1].Value;
        var val = m.Groups[2].Value;
        ArgumentHandler h;
        if (arg.Length == 1) {
          var a = arg[0];
          if (!shorts.ContainsKey(a)) {
            HandleUnknownArgument(arg, val);
            return HandleResult.Handled;
          }
          h = shorts[a];
          if (!String.IsNullOrEmpty(val) && h.IsFlag) {
            throw new InvalidValueException(String.Format(CultureInfo.CurrentCulture, "Argument \"{0}\" does not except a value", arg));
          }
          else if (String.IsNullOrEmpty(val) && !h.IsFlag) {
            val = e.MoveNext() ? e.Current : val;
            if (String.IsNullOrEmpty(val)) {
              throw new InvalidValueException(String.Format(CultureInfo.CurrentCulture, "Omitted value for argument \"{0}\"", arg));
            }
          }
          UpdateHandler(h, val, arg);
          return HandleResult.Handled;
        }
        if (opts.CaseType == ArgumentCaseType.Insensitive) {
          arg = arg.ToLower();
        }
        if (!longs.ContainsKey(arg)) {
          HandleUnknownArgument(arg, val);
          return HandleResult.Handled;
        }
        h = longs[arg];
        if (String.IsNullOrEmpty(val) && !h.IsFlag) {
          throw new InvalidValueException(String.Format(CultureInfo.CurrentCulture, "Argument \"{0}\" does except a value", arg));
        }
        UpdateHandler(h, val, arg);
        return HandleResult.Handled;
      }
      return HandleResult.NotHandled;
    }

    private static void UpdateHandler(ArgumentHandler handler, string value, string arg)
    {
      try {
        handler.Assign(value);
      }
      catch (ArgumentException ex) {
        throw new ProgrammingError(ex.Message);
      }
      catch (NotSupportedException ex) {
        throw new InvalidValueException(String.Format(CultureInfo.CurrentCulture, "Wrong value type for argument \"{0}\": {1}", arg, ex.Message));
      }
      catch (GetOptException) {
        throw;
      }
      catch (TargetInvocationException ex) {
        if (ex.InnerException is GetOptException) {
          throw ex.InnerException;
        }
        if (ex.InnerException is NotSupportedException) {
          throw new InvalidValueException(String.Format(CultureInfo.CurrentCulture, "Wrong value type for argument \"{0}\": {1}", arg, ex.Message));
        }
        throw new ProgrammingError(ex.Message);
      }
      catch (Exception ex) {
        throw new ProgrammingError(ex.Message);
      }
    }


    /// <summary>
    /// Updates the parsed argument collection, but does not yet assign it back.
    /// See Also: <seealso cref="GetOpt.Parse()"/>
    /// </summary>
    /// <exception cref="ProgrammingError">You messed something up</exception>
    /// <exception cref="UnknownAttributeException">The user supplied an argument that isn't recognized from it's name. <see cref="GetOptOptions.OnUnknownArgument"/></exception>
    /// <exception cref="InvalidValueException">The user supplied a value for an argument that cannot parsed to the correct type.</exception>
    /// <exception cref="DuplicateArgumentException">The user supplied an argument more than once and the argument type does not allow this. <see cref="Argument.OnCollision"/></exception>
    /// <param name="args">Arguments to parse</param>
    public void Update(IEnumerable<string> args)
    {
      if (args == null) {
        throw new ArgumentException("args may not be null");
      }
      var e = args.GetEnumerator();
      while (e.MoveNext()) {
        var c = e.Current;
        if ((opts.AcceptPrefixType & ArgumentPrefixTypes.Dashes) != 0) {
          var hr = MaybeHandleDashArgument(c, e);
          if (hr == HandleResult.Handled) {
            continue;
          }
          if (hr == HandleResult.Stop) {
            break;
          }
        }
        if ((opts.AcceptPrefixType & ArgumentPrefixTypes.Slashes) != 0) {
          var hr = MaybeHandleSlashArgument(c, e);
          if (hr == HandleResult.Handled) {
            continue;
          }
          if (hr == HandleResult.Stop) {
            break;
          }
        }

        UpdateHandler(parameters, c, "<parameters>");
      }

      // Consume remainder
      while (e.MoveNext()) {
        UpdateHandler(parameters, e.Current, "<parameters>");
      }
    }
  }
}
