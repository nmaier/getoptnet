using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using static System.String;

namespace NMaier.GetOptNet
{
  public abstract partial class GetOpt
  {
    private static Type GetMemberType(MemberInfo info)
    {
      var pi = info as PropertyInfo;
      var fi = info as FieldInfo;

      if (pi != null) {
        if (!pi.CanWrite) {
          throw new ProgrammingErrorException(
            Format(CultureInfo.CurrentCulture,
                   "Property {0} is an argument but not assignable",
                   info.Name));
        }

        return pi.PropertyType;
      }

      if (fi != null) {
        return fi.FieldType;
      }

      throw new ProgrammingErrorException("Huh?!");
    }

    private static bool IsIList(Type aType)
    {
      if (!aType.IsGenericType) {
        return false;
      }

      if (aType.ContainsGenericParameters) {
        throw new ProgrammingErrorException("Generic type not closed!");
      }

      var gens = aType.GetGenericArguments();
      if (gens.Length != 1) {
        return false;
      }

      var genType = typeof(IList<>).MakeGenericType(gens);
      return aType.GetInterface(genType.Name) != null;
    }

    private ArgumentHandler ConstructArgumentHandler(MemberInfo info, ArgumentAttribute arg)
    {
      var memberType = GetMemberType(info);
      var min = 0;
      var max = 0;
      var multipleArgs = info.GetAttributes<MultipleArgumentsAttribute>();
      if (multipleArgs.Length == 1) {
        min = multipleArgs[0].Min;
        max = multipleArgs[0].Max;
      }

      if (memberType.IsArray) {
        return new ArrayArgumentHandler(this, info, memberType, min, max);
      }

      if (IsIList(memberType)) {
        return new ListArgumentHandler(this, info, memberType, min, max);
      }

      if (memberType == typeof(bool) || memberType.IsSubclassOf(typeof(bool))) {
        var booleanArgs = info.GetAttributes<FlagArgumentAttribute>();
        var whenSet = booleanArgs.Length == 0 || booleanArgs[0].WhenSet;
        return new FlagArgumentHandler(this, info, arg.OnCollision, arg.Required, whenSet);
      }

      if (memberType.IsEnum) {
        return new EnumArgumentHandler(this, info, memberType, arg.OnCollision, arg.Required);
      }

      if (info.HasAttribute<CountedArgumentAttribute>()) {
        return new CounterArgumentHandler(this, info, memberType, arg.Required);
      }

      return new PlainArgumentHandler(this, info, memberType, arg.OnCollision,
                                      arg.Required);
    }

    private IEnumerable<MemberInfo> GetMemberInfos()
    {
      var me = GetType();
      const BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public |
                                 BindingFlags.Instance;
      return me.GetFields(flags).Cast<MemberInfo>().Concat(me.GetProperties(flags)).ToArray();
    }

    private void Initialize()
    {
      var me = GetType();
      opts = me.GetAttribute<GetOptOptionsAttribute>();
      foreach (var info in GetMemberInfos()) {
        var paramArgs = info.GetAttributes<ParametersAttribute>();
        if (paramArgs.Length == 1) {
          if (parameters != null || info.MemberType != MemberTypes.Field &&
              info.MemberType != MemberTypes.Property) {
            throw new ProgrammingErrorException("Duplicate declaration for parameters");
          }

          var type = GetMemberType(info);
          if (type.IsArray) {
            handlers.Add(parameters = new ArrayArgumentHandler(
                           this, info, type, paramArgs[0].Min, paramArgs[0].Max));
            continue;
          }

          if (!IsIList(type)) {
            throw new ProgrammingErrorException(
              "parameters must be an array type or a list implementation");
          }

          handlers.Add(parameters = new ListArgumentHandler(
                         this, info, type, paramArgs[0].Min, paramArgs[0].Max));
        }

        var args = info.GetAttributes<ArgumentAttribute>();
        if (args.Length < 1) {
          continue;
        }

        if (opts == null || opts.AcceptPrefixType == ArgumentPrefixTypes.None) {
          throw new ProgrammingErrorException(
            "You used Prefix=None, hence there are no arguments allowed!");
        }

        var arg = args[0];
        var name = arg.Arg;
        if (IsNullOrEmpty(name)) {
          name = info.Name;
        }

        if (opts.CaseType == ArgumentCaseType.Insensitive ||
            opts.CaseType == ArgumentCaseType.OnlyLower) {
          name = name.ToLower();
        }

        if (longs.ContainsKey(name)) {
          throw new ProgrammingErrorException($"Duplicate argument {name}");
        }

        var ai = ConstructArgumentHandler(info, arg);
        longs.Add(name, ai);
        handlers.Add(ai);

        ProcessShortArguments(info, ai);
        ProcessAliases(info, ai);
      }
    }

    private void ProcessAliases(MemberInfo info, ArgumentHandler handler)
    {
      var aliases = info.GetAttributes<ArgumentAliasAttribute>();
      foreach (var alias in aliases) {
        var an = alias.Alias;
        if (opts.CaseType == ArgumentCaseType.Insensitive ||
            opts.CaseType == ArgumentCaseType.OnlyLower) {
          an = an.ToLower();
        }

        if (longs.ContainsKey(an)) {
          throw new ProgrammingErrorException(
            $"Duplicate alias argument {an}");
        }

        longs.Add(an, handler);
      }

      var shortAliases = info.GetAttributes<ShortArgumentAliasAttribute>();
      foreach (var an in shortAliases.Select(a => a.Alias)) {
        if (shorts.ContainsKey(an)) {
          throw new ProgrammingErrorException(
            $"Duplicate short argument {an}");
        }

        shorts.Add(an, handler);
      }
    }

    private void ProcessShortArguments(MemberInfo info, ArgumentHandler handler)
    {
      var shortArguments = info.GetAttributes<ShortArgumentAttribute>();
      foreach (var an in shortArguments.Select(a => a.Arg)) {
        if (shorts.ContainsKey(an)) {
          throw new ProgrammingErrorException(
            $"Duplicate short argument {an}");
        }

        shorts.Add(an, handler);
      }
    }
  }
}
