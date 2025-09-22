namespace Domain.Common.Exceptions
{
    public class NotFoundException : DomainException
    {
        public NotFoundException(string message) : base(message) { }
    }

}
