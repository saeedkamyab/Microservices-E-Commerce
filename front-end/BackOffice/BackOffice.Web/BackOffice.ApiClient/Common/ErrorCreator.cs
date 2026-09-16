using System.Net;

namespace BackOffice.ApiClient.Common;

internal class ErrorCreator
{
    internal static async Task<ApiResult> CreateErrorResultAsync(
       HttpResponseMessage response,
       CancellationToken cancellationToken)
    {
        var message = response.StatusCode switch
        {
            HttpStatusCode.BadRequest =>
                "The submitted data is invalid.",

            HttpStatusCode.NotFound =>
                "The requested resource was not found.",

            HttpStatusCode.Conflict =>
                "The requested operation conflicts with existing data.",

            HttpStatusCode.Unauthorized =>
                "You are not authenticated.",

            HttpStatusCode.Forbidden =>
                "You do not have permission to perform this operation.",

            HttpStatusCode.InternalServerError =>
                "An unexpected server error occurred.",

            _ =>
                "An unexpected error occurred."
        };

        return ApiResult.Failure(
            new ApiError(
                ((int)response.StatusCode).ToString(),
                message));
    }
}
