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
    /// Custom exception that is thrown whenever the programmer made a mistake
    /// </summary>
    public class ProgrammingError : SystemException
    {
        /// <summary>
        /// Constructs a ProgrammingError exception
        /// </summary>
        /// <param name="Message">Message associated with the exception</param>
        public ProgrammingError(string Message) : base(Message) { }
    }

    /// <summary>
    /// Base exception for GetOptNet
    /// </summary>
    public class GetOptException : Exception
    {
        /// <summary>
        /// Constructs a GetOptException exception
        /// </summary>
        /// <param name="Message">Message associated with the exception</param>
        public GetOptException(string Message) : base(Message) { }
    }

    /// <summary>
    /// Thrown when an unknown attribute is supplied by the user.
    /// <seealso cref="GetOptOptions.OnUnknownArgument"/>
    /// </summary>
    public class UnknownAttributeException : GetOptException
    {
        /// <summary>
        /// Constructs a UnknownAttributeException exception
        /// </summary>
        /// <param name="Message">Message associated with the exception</param>
        public UnknownAttributeException(string Message) : base(Message) { }
    }

    /// <summary>
    /// Thrown when the user supplied a value for an argument that isn't compatible with the argument type
    /// </summary>
    public class InvalidValueException : GetOptException
    {
        /// <summary>
        /// Constructs a InvalidValueException exception
        /// </summary>
        /// <param name="Message">Message associated with the exception</param>
        public InvalidValueException(string Message) : base(Message) { }
    }

    /// <summary>
    /// Thrown when the user supplied an argument more than once and the argument does not support this.
    /// <seealso cref="Argument.OnCollision"/>
    /// </summary>
    public class DuplicateArgumentException : GetOptException
    {
        /// <summary>
        /// Constructs a DuplicateArgumentException exception
        /// </summary>
        /// <param name="Message">Message associated with the exception</param>
        public DuplicateArgumentException(string Message) : base(Message) { }
    }

    /// <summary>
    /// Thrown when a required argument is missing
    /// </summary>
    public class RequiredOptionMissingException : GetOptException
    {
        /// <summary>
        /// Constructs a RequiredOptionMissingException exception
        /// </summary>
        /// <param name="Message">Message associated with the exception</param>
        public RequiredOptionMissingException(string Message) : base(Message) { }
        internal RequiredOptionMissingException(ArgumentHandler aOption) : this(String.Format("Required option {0} wasn't specified", aOption.Name.ToUpper())) { }
    }
}