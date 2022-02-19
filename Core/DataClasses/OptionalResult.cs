namespace Core.DataClasses
{
    public class OptionalResult<T> : ExceptionalResult
    {
        public OptionalResult(bool isSuccess = true, string exceptionMessage = null)
            : base(isSuccess, exceptionMessage)
        {
        }

        public OptionalResult(T value)
            : base()
        {
            this.Value = value;
        }

        public OptionalResult(ExceptionalResult exceptionalResult)
            : this(exceptionalResult.IsSuccess, exceptionalResult.ExceptionMessage)
        {
        }

        public T Value { get; }
    }
}
