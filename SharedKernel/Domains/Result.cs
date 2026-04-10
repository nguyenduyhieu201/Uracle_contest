using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedKernel.Domains
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public string? Message { get; }
        public T? Value { get; }
        public ErrorCode ErrorCode { get;  }
        public bool IsFail => !IsSuccess;

        private Result(bool isSuccess, string? error, T? value)
        {
            IsSuccess = isSuccess;
            Message = error;
            Value = value;
            ErrorCode = ErrorCode.None;
        }
        private Result(bool isSuccess, string? error, T? value, ErrorCode errorCode)
        {
            IsSuccess = isSuccess;
            Message = error;
            Value = value;
            ErrorCode = errorCode;
        }

        public static Result<T> Success(T value)
        {
            return new Result<T>(true, null, value);
        }

        public static Result<T> Fail(string error, ErrorCode errorCode)
        {
            return new Result<T>(false, error, default, errorCode);
        }
    }
 }
