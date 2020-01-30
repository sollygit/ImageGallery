using System;
using System.Net;

namespace ImageGallery.Model
{
    public class ServiceException : Exception
    {
        public readonly HttpStatusCode StatusCode;

        public ServiceException()
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public ServiceException(string message) : base(message)
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public ServiceException(HttpStatusCode code, string message) : base(message)
        {
            StatusCode = code;
        }

        public ServiceException(string message, Exception inner) : base(message, inner)
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public ServiceException(HttpStatusCode code, string message, Exception inner) : base(message, inner)
        {
            StatusCode = code;
        }
    }
}
