namespace Core.DataClasses
{
    public class OptionalResult<T> : ExceptionalResult
    {
        private readonly T value;

        public OptionalResult(bool isSuccess = true, string exceptionMessage = null)
            : base(isSuccess, exceptionMessage)
        {
        }

        public OptionalResult(T value)
            : base()
        {
            this.value = value;
        }

        public OptionalResult(ExceptionalResult exceptionalResult)
            : this(exceptionalResult.IsSuccess, exceptionalResult.ExceptionMessage)
        {
        }

        public T Value => this.value;
    }
}
