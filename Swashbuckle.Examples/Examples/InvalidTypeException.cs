using System;

namespace Swashbuckle.Examples
{
    public class InvalidTypeException : ArgumentException
    {
        public override string ParamName { get; }
        public Type InvalidType { get; }
        public Type ExpectedType { get; }

        public override string Message
        {
            get
            {
                return $"Expected {ParamName} to implement {ExpectedType}. {InvalidType} does not.";
            }
        }

        public InvalidTypeException(string paramName, Type invalidType, Type expectedType)
        {
            ParamName = paramName;
            InvalidType = invalidType;
            ExpectedType = expectedType;
        }
    }
}
