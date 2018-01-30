using System;
using System.Net;
using Lykke.Common.Api.Contract.Responses;
using Refit;

namespace Lykke.Service.AssetDisclaimers.Client
{
    /// <summary>
    /// Represents error response from the Blockchain API service
    /// </summary>
    public class ErrorResponseException : Exception
    {
        public ErrorResponse Error { get; }

        public HttpStatusCode StatusCode { get; }

        public ErrorResponseException(ErrorResponse error, ApiException inner) :
            base(error.GetSummaryMessage() ?? string.Empty, inner)
        {
            Error = error;
            StatusCode = inner.StatusCode;
        }
    }
}
