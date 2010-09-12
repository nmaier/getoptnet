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

namespace NMaier.GetOptNet
{
    /// <summary>
    /// Simple command line processing. Takes commandline arguments, processes them and assign them to the appropriate
    /// fields or values in your derived class.
    /// <seealso cref="GetOptOptions"/>
    /// </summary>
    [GetOptOptions()]
    abstract public partial class GetOpt
    {
        private static Regex regDashesLong = new Regex("^\\s*--([\\w\\d_-]+)(?:=(.+)|\\s*)?$", RegexOptions.Compiled);
        private static Regex regDashesShort = new Regex("^\\s*-([\\w\\d]+)\\s*$", RegexOptions.Compiled);
        private static Regex regDashesDie = new Regex("^\\s*--\\s*$", RegexOptions.Compiled);
        private static Regex regSlashes = new Regex("^\\s*/([\\w\\d_-]+)(?:=(.+)|\\s*)?$", RegexOptions.Compiled);


        private MultipleArgumentHandler parameters;
        private Dictionary<string, ArgumentHandler> longs = new Dictionary<string, ArgumentHandler>();
        private Dictionary<char, ArgumentHandler> shorts = new Dictionary<char, ArgumentHandler>();
        private List<ArgumentHandler> handlers = new List<ArgumentHandler>();
        private GetOptOptions opts;

        /// <summary>
        /// Constructs a new command line parser instance.
        /// </summary>
        public GetOpt()
        {
            Initialize();
        }

        /// <summary>
        /// Ends the parsing of commandline arguments and assigns the parsed values to the corresponding members
        /// <seealso cref="GetOpt.Update(string[])"/>
        /// </summary>
        /// <exception cref="ProgrammingError">You messed something up</exception>
        /// <exception cref="UnknownAttributeException">The user supplied an argument that isn't recognized from it's name. <see cref="GetOptOptions.OnUnknownArgument"/></exception>
        /// <exception cref="InvalidValueException">The user supplied a value for an argument that cannot parsed to the correct type.</exception>
        /// <exception cref="DuplicateArgumentException">The user supplied an argument more than once and the argument type does not allow this. <see cref="Argument.OnCollision"/></exception>
        public void Parse()
        {
            Parse(new List<string>());
        }

        /// <summary>
        /// Ends the parsing of commandline arguments and assigns the parsed values to the corresponding members, taking a last array of arguments.
        /// <seealso cref="GetOpt.Update(string[])"/>
        /// </summary>
        /// <exception cref="ProgrammingError">You messed something up</exception>
        /// <exception cref="UnknownAttributeException">The user supplied an argument that isn't recognized from it's name. <see cref="GetOptOptions.OnUnknownArgument"/></exception>
        /// <exception cref="InvalidValueException">The user supplied a value for an argument that cannot parsed to the correct type.</exception>
        /// <exception cref="DuplicateArgumentException">The user supplied an argument more than once and the argument type does not allow this. <see cref="Argument.OnCollision"/></exception>
        /// <param name="args">Additional arguments</param>
        public void Parse(string[] args)
        {
            Parse(new List<string>(args));
        }

        /// <summary>
        /// Ends the parsing of commandline arguments and assigns the parsed values to the corresponding members
        /// <seealso cref="GetOpt.Update(string[])"/>
        /// </summary>
        /// <exception cref="ProgrammingError">You messed something up</exception>
        /// <exception cref="UnknownAttributeException">The user supplied an argument that isn't recognized from it's name. <see cref="GetOptOptions.OnUnknownArgument"/></exception>
        /// <exception cref="InvalidValueException">The user supplied a value for an argument that cannot parsed to the correct type.</exception>
        /// <exception cref="DuplicateArgumentException">The user supplied an argument more than once and the argument type does not allow this. <see cref="Argument.OnCollision"/></exception>
        /// <param name="args">Arguments to parse</param>
        public void Parse(IList<string> args)
        {
            Update(args);
            foreach (ArgumentHandler h in handlers)
            {
                try
                {
                    h.Finish();
                }
                catch (NotSupportedException)
                {
                    throw new InvalidValueException("Argument has a different type");
                }
            }
        }

        /// <summary>
        /// Updates the parsed argument collection, but does not yet assign it back.
        /// <seealso cref="GetOpt.Parse()"/>
        /// </summary>
        /// <exception cref="ProgrammingError">You messed something up</exception>
        /// <exception cref="UnknownAttributeException">The user supplied an argument that isn't recognized from it's name. <see cref="GetOptOptions.OnUnknownArgument"/></exception>
        /// <exception cref="InvalidValueException">The user supplied a value for an argument that cannot parsed to the correct type.</exception>
        /// <exception cref="DuplicateArgumentException">The user supplied an argument more than once and the argument type does not allow this. <see cref="Argument.OnCollision"/></exception>
        /// <param name="args">Arguments to parse</param>
        public void Update(string[] args)
        {
            Update(new List<string>(args));
        }
    }
}
