using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;

namespace DroppedBoxx.Web
{
    public class HttpResponse
    {
        /// <summary>
        /// Default constructor
        /// </summary>
        public HttpResponse()
        {
            Headers = new List<HttpHeader>();
        }

        /// <summary>
        /// MIME content type of response
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// Length in bytes of the response content
        /// </summary>
        public long ContentLength { get; set; }
        /// <summary>
        /// Encoding of the response content
        /// </summary>
        public string ContentEncoding { get; set; }
        /// <summary>
        /// String representation of response content
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// HTTP response status code
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
        /// <summary>
        /// Description of HTTP status returned
        /// </summary>
        public string StatusDescription { get; set; }
        /// <summary>
        /// Response content
        /// </summary>
        public Stream ResponseStream { get; set; }
        /// <summary>
        /// The URL that actually responded to the content (different from request if redirected)
        /// </summary>
        public Uri ResponseUri { get; set; }
        /// <summary>
        /// HttpWebResponse.Server
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// Headers returned by server with the response
        /// </summary>
        public IList<HttpHeader> Headers { get; private set; }

        private ResponseStatus _responseStatus = ResponseStatus.None;
        /// <summary>
        /// Status of the request. Will return Error for transport errors.
        /// HTTP errors will still return ResponseStatus.Completed, check StatusCode instead
        /// </summary>
        public ResponseStatus ResponseStatus
        {
            get
            {
                return _responseStatus;
            }
            set
            {
                _responseStatus = value;
            }
        }

        /// <summary>
        /// Transport or other non-HTTP error generated while attempting request
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
