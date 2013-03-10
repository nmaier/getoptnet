using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NMaier.GetOptNet
{
  public abstract partial class GetOpt
  {
    private List<OptInfo> CollectOptInfos(Type me, BindingFlags flags)
    {
      var options = new List<OptInfo>();


      foreach (MemberInfo[] infoArray in new MemberInfo[][] { me.GetFields(flags), me.GetProperties(flags) }) {
        foreach (MemberInfo info in infoArray) {
          string name;
          var sa = info.GetCustomAttributes(typeof(ShortArgument), true) as ShortArgument[];
          var la = info.GetCustomAttributes(typeof(Argument), true) as Argument[];
          if (la.Length == 0) {
            continue;
          }
          var longName = String.IsNullOrEmpty(la[0].Arg) ? info.Name : la[0].Arg;
          if (opts.CaseType == ArgumentCaseType.Insensitive || opts.CaseType == ArgumentCaseType.OnlyLower) {
            longName = longName.ToLower();
          }
          if (sa.Length != 0) {
            name = new string(sa[0].Arg, 1);
          }
          else {
            name = longName;
          }
          GetMemberType(info);

          var hv = la[0].HelpVar;
          if (String.IsNullOrEmpty(hv)) {
            hv = longs[longName].ElementType.Name;
          }

          var arg = la[0];
          var handler = longs[longName];
          var oi = new OptInfo(name, handler.IsFlag, arg.HelpText, hv.ToUpper(), opts.UsagePrefix);
          oi.Longs.Add(longName);

          if (sa.Length != 0) {
            oi.Shorts.Add(new string(sa[0].Arg, 1));
          }
          if (opts.UsageShowAliases == UsageAliasShowOption.Show) {
            foreach (ArgumentAlias alias in info.GetCustomAttributes(typeof(ArgumentAlias), true)) {
              var an = alias.Alias;
              if (opts.CaseType == ArgumentCaseType.Insensitive || opts.CaseType == ArgumentCaseType.OnlyLower) {
                an = an.ToLower();
              }
              oi.Longs.Add(an);
            }
            foreach (ShortArgumentAlias alias in info.GetCustomAttributes(typeof(ShortArgumentAlias), true)) {
              oi.Shorts.Add(new string(alias.Alias, 1));
            }
          }
          options.Add(oi);
        }
      }

      options.Sort();
      return options;
    }

    private string GetUsageIntro(string image)
    {
      if (!string.IsNullOrEmpty(opts.UsageIntro)) {
        return opts.UsageIntro;
      }
      if (parameters == null) {
        return string.Format("Usage: {0} [OPTION] [...]", image);
      }
      var paramInfo = (parameters.Info.GetCustomAttributes(typeof(Parameters), false) as Parameters[])[0];
      if (parameters.Min == 1 && parameters.Min == parameters.Max) {
        return string.Format("Usage: {0} [OPTION] [...] {1}", image, paramInfo.HelpVar);
      }
      if (parameters.Min == 2 && parameters.Min == parameters.Max) {
        return string.Format("Usage: {0} [OPTION] [...] {1} {1}", image, paramInfo.HelpVar);
      }
      if (parameters.Min > 0 && parameters.Min == parameters.Max) {
        return string.Format("Usage: {0} [OPTION] [...] {1}*{2}", image, paramInfo.HelpVar, parameters.Min);
      }
      return string.Format("Usage: {0} [OPTION] [...] {1} {1} ...", image, paramInfo.HelpVar);
    }


    /// <summary>
    /// Assemble Usage.
    /// </summary>
    /// <param name="width">Maximal width of a line in the usage string</param>
    /// <returns>Usage</returns>
    public string AssembleUsage(int width)
    {
      var me = GetType();
      var NL = Environment.NewLine;
      var flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;

      var rv = new StringBuilder();

      var image = new FileInfo(Assembly.GetEntryAssembly().Location).Name;
      rv.Append(GetUsageIntro(image));
      rv.Append(NL);
      rv.Append(NL);
      rv.Append("Options:");
      rv.Append(NL);

      var options = CollectOptInfos(me, flags);

      var maxLine = (long)width * 3 / 5;
      var maxArg = width / 4;

      foreach (OptInfo o in options) {
        var len = o.Argtext.Length + 3;
        if (len <= maxLine) {
          maxArg = Math.Max(maxArg, len);
        }
      }

      foreach (OptInfo o in options) {
        rv.Append(o.Argtext);
        var len = o.Argtext.Length;
        if (len > maxLine) {
          rv.Append(NL);
          len = 0;
        }
        rv.Append(new string(' ', maxArg - len));

        len = width - maxArg;

        var words = new Queue<string>(o.Helptext.Split(new char[] { ' ', '\t' }));
        while (words.Count != 0) {
          var w = words.Dequeue() + " ";
          if (len < w.Length) {
            rv.Append(NL);
            rv.Append(new string(' ', maxArg));
            len = width - maxArg;
          }
          rv.Append(w);
          len -= w.Length;
        }
        rv.Append(NL);
      }
      if (!String.IsNullOrEmpty(opts.UsageEpilog)) {
        rv.Append(NL);
        rv.Append(opts.UsageEpilog);
      }
      rv.Append(NL);
      return rv.ToString();
    }

    /// <summary>
    /// Print the usage to the allocated console (stdout).
    /// </summary>
    public void PrintUsage()
    {
      var ww = 80;
      try {
        if (Console.WindowWidth > 0) {
          ww = Console.WindowWidth;
        }
      }
      catch (IOException) {
      }
      catch (ArgumentOutOfRangeException) {
      }

      Console.Write(AssembleUsage(ww));
    }
  }
}
