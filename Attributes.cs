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

namespace NMaier.GetOptNet
{
    /// <summary>
    /// Defines how a duplicate value for an argument is handled
    /// </summary>
    public enum ArgumentCollision
    {
        /// <summary>
        /// Latter value should overwrite current
        /// </summary>
        Overwrite,
        /// <summary>
        /// Latter value is ignored
        /// </summary>
        Ignore,
        /// <summary>
        /// An exception is thrown
        /// </summary>
        Throw
    }

    /// <summary>
    /// Define an argument.
    /// </summary>
    /// <seealso cref="ArgumentAlias"/>
    /// <seealso cref="ShortArgument"/>
    /// <seealso cref="ShortArgumentAlias"/>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class Argument : Attribute
    {
        private string arg = "";
        private string helptext = "";
        private string helpvar = "";
        private ArgumentCollision collision = ArgumentCollision.Ignore;

        /// <summary>
        /// Default constructor. Will use the member's name as argument name
        /// </summary>
        public Argument() { }

        /// <summary>
        /// Constructor. Takes a custom name for the argument name.
        /// </summary>
        /// <param name="aArg">Custom argument name</param>
        public Argument(string aArg)
        {
            if (String.IsNullOrEmpty(aArg))
            {
                throw new ProgrammingError("You must specify a name");
            }
            arg = aArg;
        }

        /// <summary>
        /// Returns the name of the argument. Might be empty, indicating the member's name should be used
        /// </summary>
        /// <returns>Name of the argument</returns>
        public string GetArg() { return arg; }

        /// <summary>
        /// User-definable help text.
        /// See also: <seealso cref="GetOpt.PrintUsage()"/>, <seealso cref="GetOpt.AssembleUsage(int)"/>
        /// </summary>
        public string Helptext
        {
            get { return helptext; }
            set { helptext = value; }
        }

        /// <summary>
        /// Variable name to display in Usage.
        /// See also: <seealso cref="GetOpt.PrintUsage()"/>, <seealso cref="GetOpt.AssembleUsage(int)"/>
        /// </summary>
        public string Helpvar
        {
            get { return helpvar; }
            set { helpvar = value; }
        }

        /// <summary>
        /// Defines how to handle arguments the user supplied more than once.
        /// Has no meaning for "flag" and array/list attributes. 
        /// </summary>
        public ArgumentCollision OnCollision
        {
            get { return collision; }
            set { collision = value; }
        }
    }

    /// <summary>
    /// Defines a short argument.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ShortArgument : Attribute
    {
        private char arg;

        /// <summary>
        /// Constructor. Supply the short argument name.
        /// </summary>
        /// <param name="aArg">Argument name</param>
        public ShortArgument(char aArg)
        {
            if (String.IsNullOrEmpty(new string(aArg, 1).Trim()))
            {
                throw new ProgrammingError("You must specify a name");
            }
            arg = aArg;
        }

        /// <summary>
        /// Returns the assigned argument name. Is always set.
        /// </summary>
        /// <returns>Argument name</returns>
        public char GetArg() { return arg; }
    }

    /// <summary>
    /// Defines an alias name for an argument.
    /// See also: <seealso cref="GetOptOptions.UsageShowAliases"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class ArgumentAlias : Attribute
    {
        private string alias;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aAlias">Name of the alias</param>
        public ArgumentAlias(string aAlias)
        {
            if (String.IsNullOrEmpty(aAlias))
            {
                throw new ProgrammingError("You must specify a name");
            }
            alias = aAlias;
        }

        /// <summary>
        /// Returns the assigned alias
        /// </summary>
        /// <returns>Alias</returns>
        public string GetAlias() { return alias; }
    }

    /// <summary>
    /// Defines an short alias name for an argument.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = true)]
    public class ShortArgumentAlias : Attribute
    {
        private char alias;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="aAlias">Alias</param>
        public ShortArgumentAlias(char aAlias)
        {
            if (String.IsNullOrEmpty(new string(aAlias, 1).Trim()))
            {
                throw new ProgrammingError("You must specify a name");
            }
            alias = aAlias;
        }

        /// <summary>
        /// Returns the assigned alias
        /// </summary>
        /// <returns>Alias</returns>
        public char GetAlias() { return alias; }
    }

    /// <summary>
    /// Specifies that an argument will not take a value, but instead will count the number of occurances in the user supplied arguments.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class Counted : Attribute { }

    /// <summary>
    /// Specifies the destination of any non-argument strings (aka. parameters) the user supplies.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class Parameters : Attribute { }


    /// <summary>
    /// Specifies which type of arguments to support. Either unix-esque using dashes, windows-esque using slashes or both.
    /// None specifies that no arguments at all will be accepted. Instead only parameters are populated.
    /// </summary>
    [FlagsAttribute]
    public enum ArgumentPrefixType : ushort
    {
        /// <summary>
        /// No arguments at all.
        /// </summary>
        None = 0,

        /// <summary>
        /// Use unix-esque "dashed" arguments
        /// </summary>
        Dashes = 1,

        /// <summary>
        /// Use windows-esque "slashed" arguments
        /// </summary>
        Slashes = 2,

        /// <summary>
        /// Use both types intermixed.
        /// </summary>
        Both = 3
    }

    /// <summary>
    /// Defines the case the user may specify arguments in
    /// </summary>
    public enum ArgumentCaseType
    {
        /// <summary>
        /// Use argument name character case as defined in the program (aka. case sensitive)
        /// </summary>
        AsDefined,

        /// <summary>
        /// Use only lower case argument names
        /// </summary>
        OnlyLower,

        /// <summary>
        /// User may specify arguments using any case.
        /// </summary>
        Insensitive
    }

    /// <summary>
    /// Specifies what to do when an unknown (unspecified) argument is encountered.
    /// </summary>
    public enum UnknownArgumentsAction
    {
        /// <summary>
        /// Argument should be ignored
        /// </summary>
        Ignore,

        /// <summary>
        /// An exception should be thrown. That is an UnknownArgumentException
        /// </summary>
        Throw,

        /// <summary>
        /// Place both argument name and value (if any) in Parameters
        /// </summary>
        PlaceInParameters
    }

    /// <summary>
    /// Defines if aliases should be shown in Usage
    /// </summary>
    public enum UsageAliasShowOption
    {
        /// <summary>
        /// Show aliases
        /// </summary>
        Show,

        /// <summary>
        /// Omit aliases
        /// </summary>
        Omit
    }

    /// <summary>
    /// Defines Options for GetOpt derived classes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class GetOptOptions : Attribute
    {
        private ArgumentPrefixType apt = ArgumentPrefixType.Both;
        private ArgumentCaseType act = ArgumentCaseType.Insensitive;
        private ArgumentPrefixType upt = ArgumentPrefixType.Dashes;
        private UnknownArgumentsAction uaa = UnknownArgumentsAction.Throw;
        private UsageAliasShowOption aso = UsageAliasShowOption.Omit;

        private string usageIntro = "";
        private string usageEpilog = "";

        /// <summary>
        /// Constructor.
        /// </summary>
        public GetOptOptions() { }

        /// <summary>
        /// Defines which argument type to accept
        /// </summary>
        public ArgumentPrefixType AcceptPrefixType
        {
            get { return apt; }
            set { apt = value; }
        }

        /// <summary>
        /// Defines the character case the user may specify arguments in
        /// </summary>
        public ArgumentCaseType CaseType
        {
            get { return act; }
            set { act = value; }
        }

        /// <summary>
        /// Defines how to handle unknown arguments
        /// </summary>
        public UnknownArgumentsAction OnUnknownArgument
        {
            get { return uaa; }
            set { uaa = value; }
        }

        /// <summary>
        /// Defines wether to show or omit aliases in Usage
        /// </summary>
        public UsageAliasShowOption UsageShowAliases
        {
            get { return aso; }
            set { aso = value; }
        }

        /// <summary>
        /// Defines what argument type to use in Usage. Cannot be None or Both.
        /// </summary>
        public ArgumentPrefixType UsagePrefix
        {
            get { return upt; }
            set
            {
                if (value != ArgumentPrefixType.Dashes && value != ArgumentPrefixType.Slashes)
                {
                    throw new ProgrammingError("UsagePrefix must be Dashes or Slashes");
                }
                upt = value;
            }
        }

        /// <summary>
        /// Defines the introductory text of Usage
        /// </summary>
        public string UsageIntro
        {
            get { return usageIntro; }
            set { usageIntro = value; }
        }

        /// <summary>
        /// Defines the epilog text of usage
        /// </summary>
        public string UsageEpilog
        {
            get { return usageEpilog; }
            set { usageEpilog = value; }
        }
    }
}