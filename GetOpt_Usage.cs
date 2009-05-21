/*
 * Copyright (c) 2009 Nils Maier
 * 
 * Permission is hereby granted, free of charge, to any person
 * obtaining a copy of this software and associated documentation
 * files (the "Software"), to deal in the Software without
 * restriction, including without limitation the rights to use,
 * copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the
 * Software is furnished to do so, subject to the following
 * conditions:
 * 
 * The above copyright notice and this permission notice shall be
 * included in all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
 * EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
 * OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
 * HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
 * WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
 * OTHER DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Text;
using System.Reflection;
using System.Collections.Generic;
using System.IO;

namespace NMaier.GetOptNet
{
    sealed internal class OptInfo : IComparable<OptInfo>
    {
        private string name;

        public string Name
        {
            get { return name; }
        }

        private bool flag = false;

        public bool IsFlag
        {
            get { return flag; }
        }

        private bool acceptsMultiple = false;
        public bool AcceptsMultiple
        {
            get { return acceptsMultiple; }
        }

        private string helptext = "";

        public string Helptext
        {
            get { return helptext; }
        }

        private string helpvar = "";

        public string Helpvar
        {
            get { return helpvar; }
        }

        private string argtext = "";

        private ArgumentPrefixType prefix;

        public string Argtext
        {
            get
            {
                if (String.IsNullOrEmpty(argtext))
                {
                    StringBuilder arg = new StringBuilder("   ");

                    foreach (string a in shorts)
                    {
                        arg.Append(prefix == ArgumentPrefixType.Dashes ? "-" : "/");
                        if (flag)
                        {
                            arg.AppendFormat("{0}, ", a, helpvar);
                        }
                        else
                        {
                            arg.AppendFormat("{0} {1}, ", a, helpvar);
                        }
                    }
                    foreach (string a in longs)
                    {
                        arg.Append(prefix == ArgumentPrefixType.Dashes ? "--" : "/");
                        if (flag)
                        {
                            arg.AppendFormat("{0}, ", a);
                        }
                        else
                        {
                            arg.AppendFormat("{0}={1}, ", a, helpvar);
                        }
                    }
                    if (acceptsMultiple)
                    {
                        arg.Append("..., ");
                    }
                    arg.Remove(arg.Length - 2, 2);
                    argtext = arg.ToString();
                }
                return argtext;
            }
        }

        List<string> shorts = new List<string>();

        public List<string> Shorts
        {
            get { return shorts; }
        }

        List<string> longs = new List<string>();

        public List<string> Longs
        {
            get { return longs; }
        }

        public OptInfo(string aName, bool aFlag, bool aAcceptsMultiple, string aHelptext, string aHelpvar, ArgumentPrefixType aPrefix)
        {
            name = aName;
            flag = aFlag;
            helptext = aHelptext;
            helpvar = aHelpvar;
            prefix = aPrefix;
        }

        public int CompareTo(OptInfo other)
        {
            return name.CompareTo(other.name);
        }
    }
    abstract public partial class GetOpt
    {

        /// <summary>
        /// Print the usage to the allocated console (stdout).
        /// </summary>
        public void PrintUsage()
        {
            int ww = 80;
            try
            {
                if (Console.WindowWidth > 0)
                {
                    ww = Console.WindowWidth;
                }
            }
            catch (Exception)
            {
                // no op
            }
            Console.Write(AssembleUsage(ww));
        }

        /// <summary>
        /// Assemble Usage.
        /// </summary>
        /// <param name="width">Maximal width of a line in the usage string</param>
        /// <returns>Usage</returns>
        public string AssembleUsage(int width)
        {
            Type me = GetType();
            string nl = Environment.NewLine;
            BindingFlags flags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance;

            StringBuilder rv = new StringBuilder();

            if (!String.IsNullOrEmpty(opts.UsageIntro))
            {
                rv.Append(opts.UsageIntro);
            }
            else
            {
                rv.AppendFormat("Usage: {0} [OPTION] [...] parameters ...", new FileInfo(Assembly.GetEntryAssembly().Location).Name);
            }
            rv.Append(nl);
            rv.Append(nl);

            List<OptInfo> options = new List<OptInfo>();


            foreach (MemberInfo[] infoArray in new MemberInfo[][] { me.GetFields(flags), me.GetProperties(flags) })
            {
                foreach (MemberInfo info in infoArray)
                {
                    string name;
                    ShortArgument[] sa = info.GetCustomAttributes(typeof(ShortArgument), true) as ShortArgument[];
                    Argument[] la = info.GetCustomAttributes(typeof(Argument), true) as Argument[];
                    if (la.Length == 0)
                    {
                        continue;
                    }
                    string longName = String.IsNullOrEmpty(la[0].GetArg()) ? info.Name : la[0].GetArg();
                    if (opts.CaseType == ArgumentCaseType.Insensitive || opts.CaseType == ArgumentCaseType.OnlyLower)
                    {
                        longName = longName.ToLower();
                    }
                    if (sa.Length != 0)
                    {
                        name = new string(sa[0].GetArg(), 1);
                    }
                    else
                    {
                        name = longName;
                    }
                    Type memberType = getMemberType(info);

                    string hv = la[0].Helpvar;
                    if (String.IsNullOrEmpty(hv))
                    {
                        hv = longs[longName].ElementType.Name;
                    }

                    Argument arg = la[0];
                    ArgumentHandler handler = longs[longName];
                    OptInfo oi = new OptInfo(name, handler.IsFlag, handler.AcceptsMultiple, arg.Helptext, hv.ToUpper(), opts.UsagePrefix);
                    oi.Longs.Add(longName);
                    
                    if (sa.Length != 0)
                    {
                        oi.Shorts.Add(new string(sa[0].GetArg(), 1));
                    }
                    if (opts.UsageShowAliases == UsageAliasShowOption.Show)
                    {
                        foreach (ArgumentAlias alias in info.GetCustomAttributes(typeof(ArgumentAlias), true))
                        {
                            string an = alias.GetAlias();
                            if (opts.CaseType == ArgumentCaseType.Insensitive || opts.CaseType == ArgumentCaseType.OnlyLower)
                            {
                                an = an.ToLower();
                            }                            
                            oi.Longs.Add(an);
                        }
                        foreach (ShortArgumentAlias alias in info.GetCustomAttributes(typeof(ShortArgumentAlias), true))
                        {
                            oi.Shorts.Add(new string(alias.GetAlias(), 1));
                        }
                    }
                    options.Add(oi);
                }
            }

            options.Sort();

            int maxLine = width * 3 / 5;
            int maxArg = width / 4;

            foreach (OptInfo o in options)
            {
                int len = o.Argtext.Length + 3;
                if (len <= maxLine) {
                    maxArg = Math.Max(maxArg, len);
                }
            }
            
            foreach (OptInfo o in options) {
                rv.Append(o.Argtext);
                int len = o.Argtext.Length;
                if (len > maxLine) {
                    rv.Append(nl);
                    len = 0;
                }
                rv.Append(new string(' ', maxArg - len));

                len = width - maxArg;

                Queue<string> words = new Queue<string>(o.Helptext.Split(new char[] { ' ', '\t' }));
                while (words.Count != 0)
                {
                    string w = words.Dequeue() + " ";
                    if (len < w.Length)
                    {
                        rv.Append(nl);
                        rv.Append(new string(' ', maxArg));
                        len = width - maxArg;
                    }
                    rv.Append(w);
                    len -= w.Length;
                }
                rv.Append(nl);
            }
            if (!String.IsNullOrEmpty(opts.UsageEpilog))
            {
                rv.Append(nl);
                rv.Append(opts.UsageEpilog);
            }
            rv.Append(nl);
            return rv.ToString();
        }
    }
}