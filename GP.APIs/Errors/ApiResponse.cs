
namespace GP.APIs.Errors
{
    public class ApiResponse
    { 
        public int  StatusCode { get; set; }
        public string? Message { get; set; }
        public ApiResponse(int statusCode , string? message= null) {

            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(StatusCode);
        }

        public string? GetDefaultMessageForStatusCode(int?  statusCode)
        {
            return StatusCode switch
            {
                400 => "bad request",
                401 => "you're not authorized",
                500 => "internal server error",
                404 => "not found",
                _=> null,

            };
        }
    }
}
