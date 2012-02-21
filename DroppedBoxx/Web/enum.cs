using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace DroppedBoxx.Web
{

    /// <summary>
    /// HTTP method to use when making requests
    /// </summary>
    public enum Method
    {
        GET,
        POST,
        PUT,
        DELETE,
        HEAD,
        OPTIONS
    }

    ///<summary>
    /// Types of parameters that can be added to requests
    ///</summary>
    public enum ParameterType
    {
        Cookie,
        GetOrPost,
        UrlSegment,
        HttpHeader,
        RequestBody
    }

    /// <summary>
    /// Status for responses (surprised?)
    /// </summary>
    public enum ResponseStatus
    {
        None,
        Completed,
        Error
    }

}
