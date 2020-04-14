using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using static System.Reflection.Assembly;
using static System.String;

namespace NMaier.GetOptNet
{
  public abstract partial class GetOpt
  {
    internal static string FormatEnum(Type etype)
    {
      var names = Enum.GetNames(etype).Select(e => e.ToLowerInvariant()).ToArray();
      return $"<{Join(", ", names)}>";
    }

    private static string MakeHelpVar(Type etype)
    {
      switch (etype) {
      case Type t when t == typeof(bool):
        return "BOOL";
      case Type t when t == typeof(ushort):
        return "USHORT";
      case Type t when t == typeof(int):
        return "INT";
      case Type t when t == typeof(uint):
        return "UINT";
      case Type t when t == typeof(long):
        return "LONG";
      case Type t when t == typeof(ulong):
        return "ULONG";
      case Type t when t == typeof(DirectoryInfo):
        return "DIRECTORY";
      case Type t when t == typeof(FileInfo):
        return "FILE";
      case Type t when t == typeof(string):
        return "...";
      default:
        return etype.IsEnum ? FormatEnum(etype) : etype.Name.ToUpper(CultureInfo.CurrentUICulture);
      }
    }

    /// <summary>
    ///   Assemble Usage.
    /// </summary>
    /// <param name="width">Maximal width of a line in the usage string</param>
    /// <param name="category">Show items for this category only</param>
    /// <param name="fixedWidthFont">Set to true when you intent to display the resulting message using a fixed width font</param>
    /// <param name="introAndEpilogue">Also add the usage intro and epilogue</param>
    /// <param name="includeCommands">Include commands as well</param>
    /// <returns>Usage</returns>
    [PublicAPI]
    public virtual string AssembleUsage(int width, HelpCategory category = HelpCategory.Basic, bool fixedWidthFont = true, bool introAndEpilogue = true, bool includeCommands = true)
    {
      var nl = Environment.NewLine;
      var rv = new StringBuilder();
      var assembly = GetEntryAssembly() ??
                     GetCallingAssembly();
      var image = new FileInfo(assembly.Location).Name;

      if (introAndEpilogue) {
        var intro = GetUsageIntro(image, Empty);
        if (!IsNullOrEmpty(intro)) {
          rv.Append(intro);
          rv.Append(nl);
        }
      }

      if (includeCommands && commands.Count > 0) {
        rv.Append(nl);
        rv.Append("Commands:");
        rv.Append(nl);
        var maxname = commands.Keys.Max(e => e.Length);
        foreach (var cmd in commands.ToArray().OrderBy(e => e.Key, StringComparer.OrdinalIgnoreCase)) {
          rv.Append($"  {cmd.Key.PadRight(maxname)} - {cmd.Value.GetUsageIntro(cmd.Value.opts.UsageIntro, cmd.Key)}{nl}");
        }

        rv.Append(nl);
      }

      var options = CollectOptInfos(category);
      if (options.Count > 0) {
        rv.Append(nl);
        rv.Append("Options:");
        rv.Append(nl);

        var maxLine = (long)width / 2;
        var maxArg = width / 4;
        maxArg = options.Count > 0
          ? maxArg
          : Math.Max((from o in options
                      let len = o.Argtext.Length + 3
                      where len <= maxLine
                      select len).Max(), maxArg);
        foreach (var o in options) {
          rv.Append(o.Argtext);
          var len = o.Argtext.Length;
          if (!fixedWidthFont || len > maxLine) {
            rv.Append(nl);
            len = 0;
          }

          rv.Append(new string(' ', Math.Max(1, maxArg - len)));

          len = width - maxArg;
          var words = new Queue<string>(o.Helptext.Split(' ', '\t'));
          while (words.Count != 0) {
            var w = words.Dequeue() + " ";
            if (len < w.Length) {
              rv.Append(nl);
              rv.Append(new string(' ', maxArg));
              len = width - maxArg;
            }

            rv.Append(w);
            len -= w.Length;
          }

          rv.Append(nl);
        }
      }

      if (introAndEpilogue && !IsNullOrEmpty(opts.UsageEpilog)) {
        rv.Append(nl);
        rv.Append(opts.UsageEpilog);
      }

      rv.Append(nl);
      return rv.ToString();
    }

    /// <summary>
    ///   Print the usage to the allocated console (stdout).
    /// </summary>
    [PublicAPI]
    public void PrintUsage(HelpCategory category = HelpCategory.Basic)
    {
      var consoleWidth = 80;
      try {
        if (Console.WindowWidth > 0) {
          consoleWidth = Console.WindowWidth;
        }
      }
      catch (IOException) {
        // no op
      }
      catch (ArgumentOutOfRangeException) {
        // no op
      }

      Console.Write(AssembleUsage(consoleWidth, category));
    }

    private IList<OptInfo> CollectOptInfos(HelpCategory category)
    {
      var options = new List<OptInfo>();
      foreach (var info in GetMemberInfos()) {
        var shortArgs = info.GetAttributes<ShortArgumentAttribute>();
        var longArgs = info.GetAttributes<ArgumentAttribute>();
        if (longArgs.Length == 0) {
          continue;
        }

        var longArg = longArgs[0];
        if (longArg.Category > category) {
          continue;
        }

        var longName = IsNullOrEmpty(longArg.Arg)
          ? info.Name
          : longArg.Arg;
        if (opts.CaseType == ArgumentCaseType.Insensitive ||
            opts.CaseType == ArgumentCaseType.OnlyLower) {
          longName = longName.ToLower();
        }

        var name = shortArgs.Length != 0 ? new string(shortArgs[0].Arg, 1) : longName;

        GetMemberType(info);

        var helpVar = longArgs[0].HelpVar?.ToUpper(CultureInfo.CurrentUICulture);
        if (IsNullOrEmpty(helpVar)) {
          helpVar = MakeHelpVar(longs[longName].ElementType);
        }

        var arg = longArgs[0];
        var handler = longs[longName];
        // XXX: need to implement multi args
        var optInfo = new OptInfo(name, handler.IsFlag, arg.HelpText, helpVar, opts.UsagePrefix, false);
        optInfo.Longs.Add(longName);

        if (shortArgs.Length != 0) {
          optInfo.Shorts.Add(new string(shortArgs[0].Arg, 1));
        }

        if (opts.UsageShowAliases == UsageAliasShowOption.Show) {
          var aliases = info.GetAttributes<ArgumentAliasAttribute>();
          foreach (var alias in aliases) {
            var an = alias.Alias;
            if (opts.CaseType == ArgumentCaseType.Insensitive ||
                opts.CaseType == ArgumentCaseType.OnlyLower) {
              an = an.ToLower();
            }

            optInfo.Longs.Add(an);
          }

          var shortAliases = info.GetAttributes<ShortArgumentAliasAttribute>();
          foreach (var alias in shortAliases) {
            optInfo.Shorts.Add(new string(alias.Alias, 1));
          }
        }

        options.Add(optInfo);
      }

      options.Sort();

      if (parameters == null) {
        return options;
      }

      var paramInfo = parameters.Info.GetAttribute<ParametersAttribute>();
      var paramVar = paramInfo.HelpVar?.ToUpper(CultureInfo.CurrentUICulture);
      if (IsNullOrEmpty(paramVar)) {
        paramVar = MakeHelpVar(parameters.ElementType);
      }

      OptInfo item;
      switch (parameters.Min) {
      case 1 when parameters.Min == parameters.Max:
        item = new OptInfo(paramVar, false, "Positional parameter", Empty,
                           ArgumentPrefixTypes.None, false);
        break;
      default:
        item = new OptInfo(paramVar, false, "Positional parameters", Empty,
                           ArgumentPrefixTypes.None, true);
        break;
      }

      item.Longs.Add(paramVar);

      options.Add(item);
      return options;
    }

    protected virtual string GetUsageIntro(string image, string command)
    {
      if (!IsNullOrEmpty(opts.UsageIntro)) {
        return opts.UsageIntro;
      }

      if (!IsNullOrEmpty(command)) {
        image = $"{image} {command}";
      }

      if (parameters == null && commands.Count <= 0) {
        return $"Usage: {image} [OPTION] [...]";
      }

      if (commands.Count > 0) {
        return $"Usage: {image} [OPTION] [...] <command> [COMMAND OPTION] [...]";
      }

      if (parameters == null) {
        return $"Usage: {image} [OPTION] [...]";
      }

      var paramInfo = parameters.Info.GetAttribute<ParametersAttribute>();
      switch (parameters.Min) {
      case 1 when parameters.Min == parameters.Max:
        return $"Usage: {image} [OPTION] [...] {paramInfo.HelpVar}";
      case 2 when parameters.Min == parameters.Max:
        return Format(
          "Usage: {0} [OPTION] [...] {1} {1}", image, paramInfo.HelpVar);
      default:
        if (parameters.Min > 0 && parameters.Min == parameters.Max) {
          return $"Usage: {image} [OPTION] [...] {paramInfo.HelpVar}*{parameters.Min}";
        }

        return Format(
          "Usage: {0} [OPTION] [...] {1} {1} ...", image, paramInfo.HelpVar);
      }
    }
  }
}
