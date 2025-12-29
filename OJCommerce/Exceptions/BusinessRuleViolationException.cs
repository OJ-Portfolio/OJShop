namespace OJCommerce.Exceptions
{
    public class BusinessRuleViolationException : Exception
    {
        public BusinessRuleViolationException(string message) : base(message) { }
    }
}
