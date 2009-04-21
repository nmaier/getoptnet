using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NMaier.GetOptNet
{
    abstract public partial class GetOpt
    {
        private void UpdateHandler(ArgumentHandler handler, string value, string arg)
        {
            try
            {
                handler.Assign(value);
            }
            catch (ArgumentException ex)
            {
                throw new ProgrammingError(ex.Message);
            }
            catch (NotSupportedException ex)
            {
                throw new InvalidValueException(String.Format("Wrong value type for argument \"{0}\": {1}", arg, ex.Message));
            }
            catch (Exception ex)
            {
                throw new ProgrammingError(ex.Message);
            }
        }
        public void Update(IList<string> args)
        {
            if (args == null)
            {
                throw new ArgumentException("args may not be null");
            }
            IEnumerator<string> e = args.GetEnumerator();
            while (e.MoveNext())
            {
                string c = e.Current;
                if ((opts.AcceptPrefix & ArgumentPrefixType.Dashes) != 0)
                {
                    if (regDashesDie.IsMatch(c))
                    {
                        break;
                    }
                    Match m = regDashesLong.Match(c);
                    if (m.Success)
                    {
                        string arg = m.Groups[1].Value;
                        string val = m.Groups[2].Value;
                        if (!longs.ContainsKey(arg))
                        {
                            throw new UnknownAttributeException(String.Format("There is no attribute \"{0}\"", arg));
                        }
                        ArgumentHandler h = longs[arg];
                        if (!String.IsNullOrEmpty(val) && h.IsFlag)
                        {
                            throw new InvalidValueException(String.Format("Argument \"{0}\" does not except a value", arg));
                        }
                        else if (String.IsNullOrEmpty(val) && !h.IsFlag)
                        {
                            throw new InvalidValueException(String.Format("Omitted value for argument \"{0}\"", arg));
                        }
                        UpdateHandler(h, val, arg);

                        continue;
                    }
                    m = regDashesShort.Match(c);
                    if (m.Success)
                    {
                        string arg = m.Groups[1].Value;
                        char[] singles = arg.ToCharArray();
                        for (int i = 0, l = singles.Length; i < l; ++i)
                        {
                            if (!shorts.ContainsKey(singles[i]))
                            {
                                throw new UnknownAttributeException(String.Format("There is no attribute \"{0}\"", arg));
                            }
                            ArgumentHandler h = shorts[singles[i]];
                            if (!h.IsFlag)
                            {
                                string val = null;
                                // Consume rest of the string as argument
                                if (i != l - 1)
                                {
                                    val = arg.Substring(++i);
                                }

                                if (String.IsNullOrEmpty(val) && e.MoveNext())
                                {
                                    val = e.Current;
                                }

                                if (String.IsNullOrEmpty(val))
                                {
                                    throw new InvalidValueException(String.Format("Omitted value for argument \"{0}\"", singles[i]));
                                }
                                UpdateHandler(h, val, new string(singles[i], 1));
                                break; // We consumed the rest
                            }
                            UpdateHandler(h, null, new string(singles[i], 1));
                            continue;
                        }
                        continue;
                    }
                }
                if ((opts.AcceptPrefix & ArgumentPrefixType.Slashes) != 0)
                {
                    Match m = regSlashes.Match(c);
                    if (m.Success)
                    {
                        string arg = m.Groups[1].Value;
                        string val = m.Groups[2].Value;
                        ArgumentHandler h;
                        if (arg.Length == 1)
                        {
                            char a = arg[0];
                            if (!shorts.ContainsKey(a))
                            {
                                throw new UnknownAttributeException(String.Format("There is no attribute \"{0}\"", arg));
                            }
                            h = shorts[a];
                            if (!String.IsNullOrEmpty(val) && h.IsFlag)
                            {
                                throw new InvalidValueException(String.Format("Argument \"{0}\" does not except a value", arg));
                            }
                            else if (String.IsNullOrEmpty(val) && !h.IsFlag)
                            {
                                val = e.MoveNext() ? e.Current : val;
                                if (String.IsNullOrEmpty(val))
                                {
                                    throw new InvalidValueException(String.Format("Omitted value for argument \"{0}\"", arg));
                                }
                            }
                            UpdateHandler(h, val, arg);
                            continue;
                        }
                        if (!longs.ContainsKey(arg))
                        {
                            throw new UnknownAttributeException(String.Format("There is no attribute \"{0}\"", arg));
                        }
                        h = longs[arg];
                        if (String.IsNullOrEmpty(val) && !h.IsFlag)
                        {
                            throw new InvalidValueException(String.Format("Argument \"{0}\" does except a value", arg));
                        }
                        UpdateHandler(h, val, arg);
                        continue;
                    }
                }
                UpdateHandler(parameters, c, "<parameters>");
            }
            while (e.MoveNext())
            {
                UpdateHandler(parameters, e.Current, "<parameters>");
            }
        }
    }
}