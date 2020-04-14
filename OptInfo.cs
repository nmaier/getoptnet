using System;
using System.Collections.Generic;
using System.Text;
using static System.String;

namespace NMaier.GetOptNet
{
  internal sealed class OptInfo : IComparable<OptInfo>
  {
    private readonly bool acceptsMultiple;

    private readonly bool flag;

    private readonly string helpVar;

    private readonly string name;

    private readonly ArgumentPrefixTypes prefix;

    private string argText = Empty;


    public OptInfo(string name, bool flag, string helpText, string helpVar, ArgumentPrefixTypes prefix,
      bool acceptsMultiple)
    {
      this.name = name;
      this.flag = flag;
      Helptext = helpText;
      this.helpVar = helpVar;
      this.prefix = prefix;
      this.acceptsMultiple = acceptsMultiple;
    }

    public string Argtext
    {
      get {
        if (!IsNullOrEmpty(argText)) {
          return argText;
        }

        var arg = new StringBuilder("   ");

        foreach (var a in Shorts) {
          arg.Append(prefix == ArgumentPrefixTypes.Dashes ? "-" : "/");
          arg.AppendFormat(flag ? "{0}, " : "{0} {1}, ", a, helpVar);
        }

        foreach (var a in Longs) {
          switch (prefix) {
          case ArgumentPrefixTypes.Both:
          case ArgumentPrefixTypes.Dashes:
            arg.Append("--");
            break;
          case ArgumentPrefixTypes.None:
            break;
          case ArgumentPrefixTypes.Slashes:
            arg.Append("/");
            break;
          default:
            throw new ArgumentOutOfRangeException();
          }

          if (flag || IsNullOrEmpty(helpVar)) {
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

        return argText;
      }
    }

    public string Helptext { get; }

    public List<string> Longs { get; } = new List<string>();

    public List<string> Shorts { get; } = new List<string>();


    public int CompareTo(OptInfo other)
    {
      if (other == null) {
        throw new ArgumentNullException(nameof(other));
      }

      return Compare(name, other.name, StringComparison.Ordinal);
    }
  }
}
