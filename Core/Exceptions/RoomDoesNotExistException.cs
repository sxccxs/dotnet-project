namespace Core.Exceptions
{
    public class RoomDoesNotExistException : Exception
    {
        public RoomDoesNotExistException()
        {
        }

        public RoomDoesNotExistException(string message)
            : base(message)
        {
        }
    }
}
