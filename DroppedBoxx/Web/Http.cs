using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.IO;
using System.Security.Cryptography.X509Certificates;

namespace DroppedBoxx.Web
{
    /// <summary>
    /// HttpWebRequest wrapper
    /// </summary>
    public class Http
    {
        private const string UnreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        /// <summary>
        /// True if this HTTP request has any HTTP parameters
        /// </summary>
        protected bool HasParameters
        {
            get
            {
                return Parameters.Any();
            }
        }

        /// <summary>
        /// True if a request body has been specified
        /// </summary>
        protected bool HasBody
        {
            get
            {
                return !string.IsNullOrEmpty(RequestBody);
            }
        }

        /// <summary>
        /// True if files have been set to be uploaded
        /// </summary>
        protected bool HasFiles
        {
            get
            {
                return Files.Any();
            }
        }

        /// <summary>
        /// System.Net.ICredentials to be sent with request
        /// </summary>
        public ICredentials Credentials { get; set; }
        /// <summary>
        /// Collection of files to be sent with request
        /// </summary>
        public IList<HttpFile> Files { get; private set; }
        /// <summary>
        /// HTTP headers to be sent with request
        /// </summary>
        public IList<HttpHeader> Headers { get; private set; }
        /// <summary>
        /// HTTP parameters (QueryString or Form values) to be sent with request
        /// </summary>
        public IList<HttpParameter> Parameters { get; set; }
        /// <summary>
        /// Proxy info to be sent with request
        /// </summary>
        public IWebProxy Proxy { get; set; }
        /// <summary>
        /// Request body to be sent with request
        /// </summary>
        public string RequestBody { get; set; }
        /// <summary>
        /// Response returned from making this request
        /// </summary>
        public HttpResponse Response { get; set; }
        /// <summary>
        /// Response stream return from making this request.
        /// </summary>
        public Stream ResponseStream { get; set; }
        /// <summary>
        /// URL to call for this request
        /// </summary>
        public Uri Url { get; set; }

        public string Method { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public Http(string method)
        {
            Headers = new List<HttpHeader>();
            Files = new List<HttpFile>();
            Parameters = new List<HttpParameter>();
            Method = method.ToUpper();
        }

        /// <summary>
        /// Execute a POST request
        /// </summary>
        public void Post()
        {
            PostPutInternal("POST");
        }

        /// <summary>
        /// Execute a PUT request
        /// </summary>
        public void Put()
        {
            PostPutInternal("PUT");
        }

        private void PostPutInternal(string method)
        {
            string url = Url.ToString();
            if (HasParameters)
            {
                var data = EncodeParameters();
                url = string.Format("{0}?{1}", url, data);
            }

            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None;
            webRequest.Method = method;
            webRequest.Timeout = 1000000;

            if (Credentials != null)
            {
                webRequest.Credentials = Credentials;
            }

            if (Proxy != null)
            {
                webRequest.Proxy = Proxy;
            }

            AppendHeaders(webRequest);

            if (HasFiles)
            {
                webRequest.ContentType = GetMultipartFormContentType();
                WriteMultipartFormData(webRequest);
            }
            else
            {
                if (HasParameters)
                {
                    webRequest.ContentType = "application/x-www-form-urlencoded";
                    RequestBody = EncodeParameters();
                }
            }

            WriteRequestBody(webRequest);
            Response = GetResponse(webRequest);
        }

        private void WriteRequestBody(HttpWebRequest webRequest)
        {
            if (HasBody)
            {
                webRequest.ContentLength = RequestBody.Length;

                var requestStream = webRequest.GetRequestStream();
                using (var writer = new StreamWriter(requestStream, Encoding.ASCII))
                {
                    writer.Write(RequestBody);
                }
            }
        }

        private string _formBoundary = "----------------------------128947758029299";
        private string GetMultipartFormContentType()
        {
            return string.Format("multipart/form-data; boundary={0}", _formBoundary);
        }

        private void WriteMultipartFormData(HttpWebRequest webRequest)
        {
            var boundary = _formBoundary;
            var encoding = Encoding.UTF8;
            webRequest.AllowWriteStreamBuffering = true;
            using (Stream formDataStream = webRequest.GetRequestStream())
            {
                foreach (var file in Files)
                {
                    var data = file.Data;
                    var length = data.Length;
                    var contentType = file.ContentType;
                    // Add just the first part of this param, since we will write the file data directly to the Stream
                    string header = string.Format("--{0}{4}Content-Disposition: form-data; name=\"{2}\"; filename=\"{1}\";{4}Content-Type: {3}{4}{4}",
                                                    boundary,
                                                    file.FileName,
                                                    file.Parameter,
                                                    contentType ?? "application/octet-stream",
                                                    Environment.NewLine);

                    formDataStream.Write(encoding.GetBytes(header), 0, header.Length);
                    // Write the file data directly to the Stream, rather than serializing it to a string.
                    formDataStream.Write(data, 0, length);
                }

                string footer = String.Format("{1}--{0}--{1}", boundary, Environment.NewLine);
                formDataStream.Write(encoding.GetBytes(footer), 0, footer.Length);
            }
        }

        private string EncodeParameters()
        {
            var querystring = new StringBuilder();
            foreach (var p in Parameters)
            {
                if (querystring.Length > 1)
                    querystring.Append("&");
                querystring.AppendFormat("{0}={1}", Uri.EscapeDataString(p.Name), Uri.EscapeDataString(p.Value));
            }

            return querystring.ToString();
        }

        /// <summary>
        /// Execute a GET request
        /// </summary>
        public void Get()
        {
            GetStyleMethodInternal("GET");
        }

        public void GetAndSaveFile(string fileSavePath)
        {
            GetStyleMethodInternal("GET", fileSavePath);
        }

        /// <summary>
        /// Execute a HEAD request
        /// </summary>
        public void Head()
        {
            GetStyleMethodInternal("HEAD");
        }

        /// <summary>
        /// Execute an OPTIONS request
        /// </summary>
        public void Options()
        {
            GetStyleMethodInternal("OPTIONS");
        }

        /// <summary>
        /// Execute a DELETE request
        /// </summary>
        public void Delete()
        {
            GetStyleMethodInternal("DELETE");
        }

        private void GetStyleMethodInternal(string method)
        {
            GetStyleMethodInternal(method, null);
        }

        private void GetStyleMethodInternal(string method, string fileSavePath)
        {
            string url = Url.ToString();
            if (HasParameters)
            {
                if (url.EndsWith("/"))
                {
                    url = url.Substring(0, url.Length - 1);
                }
                var data = EncodeParameters();
                url = string.Format("{0}?{1}", url, data);
            }

            var webRequest = (HttpWebRequest)WebRequest.Create(url);
            webRequest.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip | DecompressionMethods.None;
            webRequest.Method = method;
            webRequest.Timeout = 1000000;

            if (Credentials != null)
            {
                webRequest.Credentials = Credentials;
            }

            if (Proxy != null)
            {
                webRequest.Proxy = Proxy;
            }

            AppendHeaders(webRequest);

            if (string.IsNullOrEmpty(fileSavePath))
            {
                Response = GetResponse(webRequest);
            }
            else
            {
                Response = GetResponseAndSaveFile(webRequest, fileSavePath);
            }
        }

        private void AppendHeaders(HttpWebRequest webRequest)
        {
            foreach (var header in Headers)
            {
                if (_restrictedHeaderActions.ContainsKey(header.Name))
                {
                    _restrictedHeaderActions[header.Name].Invoke(webRequest, header.Value);
                }
                else
                {
                    webRequest.Headers.Add(header.Name, header.Value);
                }
            }
        }

        private readonly IDictionary<string, Action<HttpWebRequest, string>> _restrictedHeaderActions
            = new Dictionary<string, Action<HttpWebRequest, string>>(StringComparer.OrdinalIgnoreCase) {
                      { "Accept",            (r, v) => r.Accept = v },
                      { "Connection",        (r, v) => r.Connection = v },           
                      { "Content-Length",    (r, v) => r.ContentLength = Convert.ToInt64(v) },
                      { "Content-Type",      (r, v) => r.ContentType = v },
                      { "Expect",            (r, v) => r.Expect = v },
                      { "Date",              (r, v) => { /* Set by system */ }},
                      { "Host",              (r, v) => { /* Set by system */ }},
                      { "If-Modified-Since", (r, v) => r.IfModifiedSince = Convert.ToDateTime(v) },
                      { "Range",             (r, v) => { throw new NotImplementedException(/* r.AddRange() */); }},
                      { "Referer",           (r, v) => r.Referer = v },
                      { "Transfer-Encoding", (r, v) => { r.TransferEncoding = v; r.SendChunked = true; } },
                      { "User-Agent",        (r, v) => r.UserAgent = v }
                  };

        private HttpResponse GetResponse(HttpWebRequest request)
        {
            var response = new HttpResponse();
            response.ResponseStatus = ResponseStatus.None;

            try
            {
                var webResponse = GetRawResponse(request);
                using (webResponse)
                {
                    response.ContentType = webResponse.ContentType;
                    response.ContentLength = webResponse.ContentLength;
                    response.ContentEncoding = webResponse.ContentEncoding;
                    response.ResponseStream = webResponse.GetResponseStream();
                    response.StatusCode = webResponse.StatusCode;
                    response.StatusDescription = webResponse.StatusDescription;
                    response.ResponseUri = webResponse.ResponseUri;
                    response.Server = webResponse.Server;
                    response.ResponseStatus = ResponseStatus.Completed;

                    using (StreamReader reader = new StreamReader(response.ResponseStream))
                    {
                        response.Content = reader.ReadToEnd();
                    }

                    foreach (var headerName in webResponse.Headers.AllKeys)
                    {
                        var headerValue = webResponse.Headers[headerName];
                        response.Headers.Add(new HttpHeader { Name = headerName, Value = headerValue });
                    }

                    webResponse.Close();
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.ResponseStatus = ResponseStatus.Error;
            }

            return response;
        }

        private HttpResponse GetResponseAndSaveFile(HttpWebRequest request, string fileSavePath)
        {
            var response = new HttpResponse();
            response.ResponseStatus = ResponseStatus.None;

            try
            {
                var webResponse = GetRawResponse(request);
                using (webResponse)
                {
                    response.ContentType = webResponse.ContentType;
                    response.ContentLength = webResponse.ContentLength;
                    response.ContentEncoding = webResponse.ContentEncoding;
                    response.StatusCode = webResponse.StatusCode;
                    response.StatusDescription = webResponse.StatusDescription;
                    response.ResponseUri = webResponse.ResponseUri;
                    response.Server = webResponse.Server;
                    response.ResponseStatus = ResponseStatus.Completed;

                    using (Stream output = File.OpenWrite(fileSavePath))
                    using (Stream input = webResponse.GetResponseStream())
                    {
                        byte[] buffer = new byte[8 * 1024];
                        int len;
                        while ((len = input.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            output.Write(buffer, 0, len);
                        }    
                        output.Close();
                    }

                    foreach (var headerName in webResponse.Headers.AllKeys)
                    {
                        var headerValue = webResponse.Headers[headerName];
                        response.Headers.Add(new HttpHeader { Name = headerName, Value = headerValue });
                    }

                    webResponse.Close();
                }
            }
            catch (Exception ex)
            {
                response.ErrorMessage = ex.Message;
                response.ResponseStatus = ResponseStatus.Error;
            }

            return response;
        }

        private HttpWebResponse GetRawResponse(HttpWebRequest request)
        {
            ServicePointManager.CertificatePolicy = new AcceptAllCertificatePolicy();

            HttpWebResponse raw = null;
            try
            {
                raw = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException ex)
            {
                if (ex.Response is HttpWebResponse)
                {
                    raw = ex.Response as HttpWebResponse;
                }
            }

            return raw;
        }

        protected string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (UnreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }

    }

    /// <summary>
    /// Internal object used to allow setting WebRequest.CertificatePolicy to 
    /// not fail on Cert errors
    /// </summary>
    internal class AcceptAllCertificatePolicy : ICertificatePolicy
    {
        public AcceptAllCertificatePolicy()
        {
        }

        public bool CheckValidationResult(ServicePoint sPoint,
           X509Certificate cert, WebRequest wRequest, int certProb)
        {
            // *** Always accept
            return true;
        }
    }
}
