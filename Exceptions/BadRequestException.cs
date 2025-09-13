namespace OrderManagementApi.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException()
        {

        }

        public BadRequestException(string Message)
            : base(Message)
        {

        }

        public BadRequestException(string Message, Exception excecaoInterna)
            : base(Message, excecaoInterna)
        {

        }
    }
}
