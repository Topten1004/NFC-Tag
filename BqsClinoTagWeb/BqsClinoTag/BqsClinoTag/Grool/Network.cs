using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BqsClinoTag.Grool
{
    public class Network
    {
        HttpRequestMessage request;

        public Network(HttpRequestMessage r)
        {
            request = r;
        }

        public static string GetBaseUrl(HttpContext context)
        {
            string url = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}";
            return url.Replace("http://", "https://");
        }

        public static string GetFullUrl(HttpContext context)
        {
            string url = $"{context.Request.Scheme}://{context.Request.Host}{context.Request.PathBase}{context.Request.Path}";
            return url.Replace("http://", "https://");
        }

        public static string GetClientIp(HttpContext context)
        {
            var result = string.Empty;

            //first try to get IP address from the forwarded header
            if (context.Request.Headers != null)
            {
                //the X-Forwarded-For (XFF) HTTP header field is a de facto standard for identifying the originating IP address of a client
                //connecting to a web server through an HTTP proxy or load balancer

                var forwardedHeader = context.Request.Headers["X-Forwarded-For"];
                if (!StringValues.IsNullOrEmpty(forwardedHeader))
                    result = forwardedHeader.FirstOrDefault();
            }

            //if this header not exists try get connection remote IP address
            if (string.IsNullOrEmpty(result) && context.Connection.RemoteIpAddress != null)
                result = context.Connection.RemoteIpAddress.ToString();

            return result;

            //if (request.Properties.ContainsKey("MS_HttpContext"))
            //{
            //    return ((IHttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            //}
            //else if (request.Properties.ContainsKey(RemoteEndpointMessageProperty.Name))
            //{
            //    RemoteEndpointMessageProperty prop = (RemoteEndpointMessageProperty)request.Properties[RemoteEndpointMessageProperty.Name];
            //    return prop.Address;
            //}
            //else if (HttpContext.Current != null)
            //{
            //    return HttpContext.Current.Request.UserHostAddress;
            //}
            //else
            //{
            //    return null;
            //}
        }

    }
}
