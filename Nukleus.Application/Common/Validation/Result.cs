using System.Diagnostics.Contracts;

namespace Nukleus.Application.Common.Validation
{
    public static class ResultFactory
    {
        public static Result<TValue> From<TValue>(TValue value)
        {
            return value;
        }
    }

    public readonly record struct Result
    {
        private readonly bool isSuccessful;

        public readonly Error Error;

        // Public Getters, extracted away from internal data
        [Pure]
        public bool IsSuccessful => isSuccessful == true;
        [Pure]
        public bool IsFaulted => isSuccessful == false;

        public Result(bool isSuccessful, Error error = default)
        {
            this.isSuccessful = isSuccessful;
            Error = error;
        }

        public static Result<TValue> From<TValue>(TValue value)
        {
            return value;
        }

        // Implicit conversion operators for return types.

        [Pure]
        public static implicit operator Result(bool isSuccess) => new Result(isSuccess);

        [Pure]
        public static implicit operator Result(Error error) => new Result(false, error);

        public void Switch(Action onValue, Action<Error> onError)
        {
            if (IsFaulted)
            {
                onError(Error);
                return;
            }

            onValue();
        }

        public async Task SwitchAsync(Func<Task> onValue, Func<Error, Task> onError)
        {
            if (IsFaulted)
            {
                await onError(Error).ConfigureAwait(false);
                return;
            }

            await onValue().ConfigureAwait(false);
        }

        public void Match(Action onValue, Action<Error> onError)
        {
            if (IsFaulted)
            {
                onError(Error);
            }

            onValue();
        }

        public async Task MatchAsync(Func<Task> onValue, Func<Error, Task> onError)
        {
            if (IsFaulted)
            {
                await onError(Error).ConfigureAwait(false);
            }

            await onValue().ConfigureAwait(false);
        }
    }
    public readonly record struct Result<TValue>
    {
        private readonly bool isSuccessful = false;
        public readonly TValue Value;
        
        public readonly Error Error;

        // Public Getters, extracted away from internal data
        [Pure]
        public bool IsSuccessful => isSuccessful == true;
        [Pure]
        public bool IsFaulted => isSuccessful == false;

        public Result(TValue value)
        {
            Value = value;
            isSuccessful = true;
        }

        public Result(Error error)
        {
            Error = error;
            isSuccessful = false;
        }

        // Implicit conversion operators for return types.

        [Pure]
        public static implicit operator Result(Result<TValue> result) => new Result(result.IsSuccessful, result.Error);

        [Pure]
        public static implicit operator Result<TValue>(TValue value) => new Result<TValue>(value);

        [Pure]
        public static implicit operator Result<TValue>(Error error) => new Result<TValue>(error);

        public void Switch(Action<TValue> onValue, Action<Error> onError)
        {
            if (IsFaulted)
            {
                onError(Error);
                return;
            }

            onValue(Value);
        }

        public async Task SwitchAsync(Func<TValue, Task> onValue, Func<Error, Task> onError)
        {
            if (IsFaulted)
            {
                await onError(Error).ConfigureAwait(false);
                return;
            }

            await onValue(Value).ConfigureAwait(false);
        }

        public TResult Match<TResult>(Func<TValue, TResult> onValue, Func<Error, TResult> onError)
        {
            if (IsFaulted)
            {
                return onError(Error);
            }

            return onValue(Value);
        }

        public async Task<TResult> MatchAsync<TResult>(Func<TValue, Task<TResult>> onValue, Func<Error, Task<TResult>> onError)
        {
            if (IsFaulted)
            {
                return await onError(Error).ConfigureAwait(false);
            }

            return await onValue(Value).ConfigureAwait(false);
        }
    }
}