namespace BackOffice.ApiClient.Common;

public sealed class ApiResult
{
    public bool IsSuccess { get; }

    public ApiError? Error { get; }

    private ApiResult(bool isSuccess, ApiError? error)
    {
        IsSuccess = isSuccess;
        Error = error;
    }

    public static ApiResult Success()
        => new(true, null);

    public static ApiResult Failure(ApiError error)
        => new(false, error);
}