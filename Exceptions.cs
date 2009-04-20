using System;
namespace NMaier.GetOptNet
{
    public class ProgrammingError : SystemException
    {
        public ProgrammingError(string Message) : base(Message) { }
    }

    public class GetOptException : Exception
    {
        public GetOptException(string Message) : base(Message) { }
    }

    public class UnknownAttributeException : GetOptException
    {
        public UnknownAttributeException(string Message) : base(Message) { }
    }

    public class InvalidValueException : GetOptException
    {
        public InvalidValueException(string Message) : base(Message) { }
    }

    public class DuplicateArgumentException : GetOptException
    {
        public DuplicateArgumentException(string Message) : base(Message) { }
    }
}