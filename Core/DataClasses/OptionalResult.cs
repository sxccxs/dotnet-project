namespace Core.DataClasses
{
    public class OptionalResult<T>
    {
        private readonly T value;

        public OptionalResult()
        {
        }

        public OptionalResult(T value)
        {
            this.value = value;
        }

        public bool HasValue
        {
            get { return this.value is not null; }
        }

        public T Value
        {
            get
            {
                return this.value is not null ? this.value :
                                            throw new InvalidOperationException("Can't get value from empty result.");
            }
        }
    }
}
