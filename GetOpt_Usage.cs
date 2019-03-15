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

    /// <summary>
    ///   Assemble Usage.
    /// </summary>
    /// <param name="width">Maximal width of a line in the usage string</param>
    /// <param name="category">Show items for this category only</param>
    /// <param name="fixedWidthFont">Set to true when you intent to display the resulting message using a fixed width font</param>
    /// <returns>Usage</returns>
    [PublicAPI]
    public string AssembleUsage(int width, HelpCategory category = HelpCategory.Basic, bool fixedWidthFont = true)
    {
      var nl = Environment.NewLine;
      var rv = new StringBuilder();
      var assembly = GetEntryAssembly() ??
                     GetCallingAssembly();
      var image = new FileInfo(assembly.Location).Name;

      rv.Append(GetUsageIntro(image));
      rv.Append(nl);
      rv.Append(nl);
      rv.Append("Options:");
      rv.Append(nl);

      var options = CollectOptInfos(category);
      var maxLine = (long)width / 2;
      var maxArg = width / 4;
      maxArg = Math.Max((from o in options
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

        rv.Append(new string(' ', maxArg - len));

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

      if (!IsNullOrEmpty(opts.UsageEpilog)) {
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
        string name;
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

        name = shortArgs.Length != 0 ? new string(shortArgs[0].Arg, 1) : longName;

        GetMemberType(info);

        var helpVar = longArgs[0].HelpVar?.ToUpper(CultureInfo.CurrentUICulture);
        if (IsNullOrEmpty(helpVar)) {
          var etype = longs[longName].ElementType;
          helpVar = etype.IsEnum ? FormatEnum(etype) : etype.Name.ToUpper(CultureInfo.CurrentUICulture);
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
      return options;
    }

    private string GetUsageIntro(string image)
    {
      if (!IsNullOrEmpty(opts.UsageIntro)) {
        return opts.UsageIntro;
      }

      if (parameters == null) {
        return $"Usage: {image} [OPTION] [...]";
      }

      var paramInfo = parameters.Info.GetAttribute<ParametersAttribute>();
      if (parameters.Min == 1 && parameters.Min == parameters.Max) {
        return $"Usage: {image} [OPTION] [...] {paramInfo.HelpVar}";
      }

      if (parameters.Min == 2 && parameters.Min == parameters.Max) {
        return Format(
          "Usage: {0} [OPTION] [...] {1} {1}", image, paramInfo.HelpVar);
      }

      if (parameters.Min > 0 && parameters.Min == parameters.Max) {
        return $"Usage: {image} [OPTION] [...] {paramInfo.HelpVar}*{parameters.Min}";
      }

      return Format(
        "Usage: {0} [OPTION] [...] {1} {1} ...", image, paramInfo.HelpVar);
    }
  }
}
