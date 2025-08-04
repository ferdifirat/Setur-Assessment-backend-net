namespace Shared.Kernel.Results
{
    public class Result<T>
    {
        public T Value { get; }
        public string Error { get; }
        public bool IsSuccess => Error is null;

        public static Result<T> Success(T value) => new(value, null);
        public static Result<T> Fail(string error) => new(default, error);

        private Result(T value, string error)
        {
            Value = value;
            Error = error;
        }
    }

    public class Result
    {
        public string Error { get; }
        public bool IsSuccess => Error is null;

        public static Result Success() => new(null);
        public static Result Fail(string error) => new(error);

        private Result(string error)
        {
            Error = error;
        }
    }
}
