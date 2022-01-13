namespace Core.Exceptions
{
    public class UserDoesNotBelongToRoomException : Exception
    {
        public UserDoesNotBelongToRoomException()
        {
        }

        public UserDoesNotBelongToRoomException(string message)
            : base(message)
        {
        }
    }
}
