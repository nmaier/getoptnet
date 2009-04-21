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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;

namespace NMaier.GetOptNet
{
    abstract public partial class GetOpt
    {
        /// <summary>
        /// Updates the parsed argument collection, but does not yet assign it back.
        /// See Also: <seealso cref="GetOpt.Parse()"/>
        /// </summary>
        /// <exception cref="ProgrammingError">You messed something up</exception>
        /// <exception cref="UnknownAttributeException">The user supplied an argument that isn't recognized from it's name. <see cref="GetOptOptions.OnUnknownArgument"/></exception>
        /// <exception cref="InvalidValueException">The user supplied a value for an argument that cannot parsed to the correct type.</exception>
        /// <exception cref="DuplicateArgumentException">The user supplied an argument more than once and the argument type does not allow this. <see cref="Argument.OnCollision"/></exception>
        /// <param name="args">Arguments to parse</param>
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
                if ((opts.AcceptType & ArgumentPrefixType.Dashes) != 0)
                {
                    if (regDashesDie.IsMatch(c))
                    {
                        break;
                    }
                    Match m = regDashesLong.Match(c);
                    if (m.Success)
                    {
                        string arg = m.Groups[1].Value;
                        if (opts.CaseType == ArgumentCaseType.Insensitive)
                        {
                            arg = arg.ToLower();
                        }

                        string val = m.Groups[2].Value;
                        if (!longs.ContainsKey(arg))
                        {
                            handleUnknownArgument(arg, val);
                            continue;
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
                        updateHandler(h, val, arg);

                        continue;
                    }
                    m = regDashesShort.Match(c);
                    if (m.Success)
                    {
                        string arg = m.Groups[1].Value;
                        char[] singles = arg.ToCharArray();
                        for (int i = 0, l = singles.Length; i < l; ++i)
                        {
                            char currentArg = singles[i];
                            if (!shorts.ContainsKey(currentArg))
                            {
                                handleUnknownArgument(new string(currentArg, 1), null);
                                continue;
                            }
                            ArgumentHandler h = shorts[currentArg];
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
                                    throw new InvalidValueException(String.Format("Omitted value for argument \"{0}\"", currentArg));
                                }
                                updateHandler(h, val, new string(currentArg, 1));
                                break; // We consumed the rest
                            }
                            updateHandler(h, null, new string(currentArg, 1));
                            continue;
                        }
                        continue;
                    }
                }
                if ((opts.AcceptType & ArgumentPrefixType.Slashes) != 0)
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
                                handleUnknownArgument(arg, val);
                                continue;

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
                            updateHandler(h, val, arg);
                            continue;
                        }
                        if (opts.CaseType == ArgumentCaseType.Insensitive)
                        {
                            arg = arg.ToLower();
                        }
                        if (!longs.ContainsKey(arg))
                        {
                            handleUnknownArgument(arg, val);
                            continue;
                        }
                        h = longs[arg];
                        if (String.IsNullOrEmpty(val) && !h.IsFlag)
                        {
                            throw new InvalidValueException(String.Format("Argument \"{0}\" does except a value", arg));
                        }
                        updateHandler(h, val, arg);
                        continue;
                    }
                }
                updateHandler(parameters, c, "<parameters>");
            }
            while (e.MoveNext())
            {
                updateHandler(parameters, e.Current, "<parameters>");
            }
        }

        private void handleUnknownArgument(string arg, string val)
        {
            switch (opts.OnUnknownArgument)
            {
                case UnknownArgumentsAction.Throw:
                    throw new UnknownAttributeException(String.Format("There is no argument with the name \"{0}\"", arg));

                case UnknownArgumentsAction.PlaceInParameters:
                    parameters.Assign(arg);
                    if (!String.IsNullOrEmpty(val))
                    {
                        parameters.Assign(val);
                    }
                    break;
            }
        }
        private void updateHandler(ArgumentHandler handler, string value, string arg)
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
            catch (GetOptException ex)
            {
                throw ex;
            }
            catch (TargetInvocationException ex)
            {
                if (ex.InnerException is GetOptException)
                {
                    throw ex.InnerException;
                }
                if (ex.InnerException is NotSupportedException)
                {
                    throw new InvalidValueException(String.Format("Wrong value type for argument \"{0}\": {1}", arg, ex.Message));
                }
                throw new ProgrammingError(ex.Message);
            }
            catch (Exception ex)
            {
                throw new ProgrammingError(ex.Message);
            }
        }
    }
}