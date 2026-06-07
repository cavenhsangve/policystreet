namespace EMS.Api.Exceptions
{
    public class DuplicateEmailException : Exception
    {
        public DuplicateEmailException() : base("An employee with this email already exists.") { }
    }
}
