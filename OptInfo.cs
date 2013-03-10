using System;
using System.Collections.Generic;
using System.Text;

namespace NMaier.GetOptNet
{
  internal sealed class OptInfo : IComparable<OptInfo>
  {
    private readonly bool acceptsMultiple = false;

    private readonly bool flag = false;

    private readonly string helpText = string.Empty;

    private readonly string helpVar = string.Empty;

    private string argText = string.Empty;

    private readonly List<string> shorts = new List<string>();

    private readonly List<string> longs = new List<string>();

    private readonly string name;

    private readonly ArgumentPrefixTypes prefix;


    public OptInfo(string name, bool flag, string helpText, string helpVar, ArgumentPrefixTypes prefix)
    {
      this.name = name;
      this.flag = flag;
      this.helpText = helpText;
      this.helpVar = helpVar;
      this.prefix = prefix;
    }

    public string Argtext
    {
      get
      {
        if (String.IsNullOrEmpty(argText)) {
          var arg = new StringBuilder("   ");

          foreach (string a in shorts) {
            arg.Append(prefix == ArgumentPrefixTypes.Dashes ? "-" : "/");
            if (flag) {
              arg.AppendFormat("{0}, ", a, helpVar);
            }
            else {
              arg.AppendFormat("{0} {1}, ", a, helpVar);
            }
          }
          foreach (string a in longs) {
            arg.Append(prefix == ArgumentPrefixTypes.Dashes ? "--" : "/");
            if (flag) {
              arg.AppendFormat("{0}, ", a);
            }
            else {
              arg.AppendFormat("{0}={1}, ", a, helpVar);
            }
          }
          if (acceptsMultiple) {
            arg.Append("..., ");
          }
          arg.Remove(arg.Length - 2, 2);
          argText = arg.ToString();
        }
        return argText;
      }
    }
    public string Helptext
    {
      get
      {
        return helpText;
      }
    }
    public List<string> Longs
    {
      get
      {
        return longs;
      }
    }
    public List<string> Shorts
    {
      get
      {
        return shorts;
      }
    }


    public int CompareTo(OptInfo other)
    {
      if (other == null) {
        throw new ArgumentNullException("other");
      }
      return name.CompareTo(other.name);
    }
  }
}
