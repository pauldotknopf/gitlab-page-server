using System;
using System.Net;

namespace GitLabPages
{
    public class ApiRequestException : Exception
    {
        public ApiRequestException(string message, HttpStatusCode statusCode)
            : base(message)
        {
            StatusCode = statusCode;
        }
        
        public HttpStatusCode StatusCode { get; set; }
    }
}