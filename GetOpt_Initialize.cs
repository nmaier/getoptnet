using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace NMaier.GetOptNet
{
  public abstract partial class GetOpt
  {
    private ArgumentHandler ConstructArgumentHandler(MemberInfo info, Argument arg)
    {
      ArgumentHandler ai;
      var memberType = GetMemberType(info);
      var min = 0;
      var max = 0;
      var margs = info.GetCustomAttributes(typeof(MultipleArguments), true) as MultipleArguments[];
      if (margs.Length == 1) {
        min = margs[0].Min;
        max = margs[0].Max;
      }
      if (memberType.IsArray) {
        ai = new ArrayArgumentHandler(this, info, memberType, min, max);
      }
      else if (isIList(memberType)) {
        ai = new IListArgumentHandler(this, info, memberType, min, max);
      }
      else if (memberType == typeof(bool) || memberType == typeof(Boolean) || memberType.IsSubclassOf(typeof(Boolean))) {
        var bargs = info.GetCustomAttributes(typeof(FlagArgument), true) as FlagArgument[];
        ai = new FlagArgumentHandler(this, info, arg.OnCollision, arg.Required, bargs.Length != 0 ? bargs[0].WhenSet : true);
      }
      else if (info.GetCustomAttributes(typeof(CountedArgument), true).Length != 0) {
        ai = new CounterArgumentHandler(this, info, memberType, arg.Required);
      }
      else {
        ai = new PlainArgumentHandler(this, info, memberType, arg.OnCollision, arg.Required);
      }
      return ai;
    }

    private static Type GetMemberType(MemberInfo info)
    {
      var pi = info as PropertyInfo;
      var fi = info as FieldInfo;

      Type memberType;
      if (pi != null) {
        if (!pi.CanWrite) {
          throw new ProgrammingError(String.Format(CultureInfo.CurrentCulture, "Property {0} is an argument but not assignable", info.Name));
        }
        memberType = pi.PropertyType;
      }
      else {
        if (fi != null) {
          memberType = fi.FieldType;
        }
        else {
          throw new ProgrammingError("WTF?!");
        }
      }
      return memberType;
    }

    private void Initialize()
    {
      var me = GetType();
      opts = me.GetCustomAttributes(typeof(GetOptOptions), true)[0] as GetOptOptions;
      var flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;
      foreach (MemberInfo[] infoArray in new MemberInfo[][] { me.GetFields(flags), me.GetProperties(flags) }) {
        foreach (MemberInfo info in infoArray) {
          var paramArgs = info.GetCustomAttributes(typeof(Parameters), true) as Parameters[];
          if (paramArgs.Length == 1) {
            if (parameters != null || (info.MemberType != MemberTypes.Field && info.MemberType != MemberTypes.Property)) {
              throw new ProgrammingError("Duplicate declaration for parameters");
            }
            var type = GetMemberType(info);
            if (type.IsArray) {
              parameters = new ArrayArgumentHandler(this, info, type, paramArgs[0].Min, paramArgs[0].Max);
            }
            else {
              if (isIList(type)) {
                parameters = new IListArgumentHandler(this, info, type, paramArgs[0].Min, paramArgs[0].Max);
              }
              else {
                throw new ProgrammingError("parameters must be an array type or a list implementation");
              }
            }
            handlers.Add(parameters);
            continue;
          }
          var args = info.GetCustomAttributes(typeof(Argument), true) as Argument[];
          if (args.Length != 1) {
            continue;
          }
          if (opts.AcceptPrefixType == ArgumentPrefixTypes.None) {
            throw new ProgrammingError("You used Prefix=None, hence there are no arguments allowed!");
          }
          var arg = args[0];
          var name = arg.Arg;
          if (String.IsNullOrEmpty(name)) {
            name = info.Name;
          }
          if (opts.CaseType == ArgumentCaseType.Insensitive || opts.CaseType == ArgumentCaseType.OnlyLower) {
            name = name.ToLower();
          }
          if (longs.ContainsKey(name)) {
            throw new ProgrammingError(String.Format("Duplicate argument {0}", name));
          }

          var ai = ConstructArgumentHandler(info, arg);
          longs.Add(name, ai);
          handlers.Add(ai);

          ProcessShortArguments(info, ai);
          ProcessAliases(info, ai);
        }
      }
    }

    private static bool isIList(Type aType)
    {
      if (!aType.IsGenericType) {
        return false;
      }
      if (aType.ContainsGenericParameters) {
        throw new ProgrammingError("Generic type not closed!");
      }
      var gens = aType.GetGenericArguments();
      if (gens.Length == 1) {
        var genType = typeof(IList<>).MakeGenericType(gens);
        if (aType.GetInterface(genType.Name) != null) {
          return true;
        }
      }
      return false;
    }

    private void ProcessAliases(MemberInfo info, ArgumentHandler ai)
    {
      foreach (ArgumentAlias alias in info.GetCustomAttributes(typeof(ArgumentAlias), true)) {
        var an = alias.Alias;
        if (opts.CaseType == ArgumentCaseType.Insensitive || opts.CaseType == ArgumentCaseType.OnlyLower) {
          an = an.ToLower();
        }
        if (longs.ContainsKey(an)) {
          throw new ProgrammingError(String.Format("Duplicate alias argument {0}", an));
        }
        longs.Add(an, ai);
      }

      foreach (ShortArgumentAlias sa in info.GetCustomAttributes(typeof(ShortArgumentAlias), true)) {
        var an = sa.Alias;
        if (shorts.ContainsKey(an)) {
          throw new ProgrammingError(String.Format("Duplicate short argument {0}", an));
        }
        shorts.Add(an, ai);
      }
    }

    private void ProcessShortArguments(MemberInfo info, ArgumentHandler ai)
    {
      foreach (ShortArgument sa in info.GetCustomAttributes(typeof(ShortArgument), true)) {
        var an = sa.Arg;
        if (shorts.ContainsKey(an)) {
          throw new ProgrammingError(String.Format("Duplicate short argument {0}", an));
        }
        shorts.Add(an, ai);
      }
    }
  }
}
