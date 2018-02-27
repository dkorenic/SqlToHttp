using System;
using System.Data.SqlTypes;
using System.IO;
using System.Net;

public partial class StoredProcedures
{
    [Microsoft.SqlServer.Server.SqlProcedure]
    public static int HttpPost(SqlString url, SqlString requestContentType, SqlString body, SqlString acceptContentType, out SqlString responseBody, out SqlString responseContentType, int timeout = 1000)
    {
        int status = HttpRequest(url.Value, "POST", requestContentType.Value, body.Value, acceptContentType.Value, out string _responseContentType, out string _responseBody, timeout);
        responseContentType = _responseContentType;
        responseBody = _responseBody;
        return status;
    }


    [Microsoft.SqlServer.Server.SqlProcedure]
    public static int HttpGet(SqlString url, SqlString requestContentType, SqlString acceptContentType, out SqlString responseBody, out SqlString responseContentType, int timeout = 1000)
    {
        int status = HttpRequest(url.Value, "GET", requestContentType.Value, null, acceptContentType.Value, out string _responseContentType, out string _responseBody, timeout);
        responseContentType = _responseContentType;
        responseBody = _responseBody;
        return status;
    }


    public static int HttpRequest(String url, String method, String requestContentType, String body, String acceptContentType, out String responseContentType, out String responseBody, int timeout = 1000)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
        request.ContentType = requestContentType;
        if (acceptContentType != null) request.Accept = acceptContentType;
        request.Method = method;
        request.Timeout = timeout;
        
        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
        {
            streamWriter.Write(body);
            streamWriter.Flush();
        }

        using (var response = (HttpWebResponse)request.GetResponse())
        {
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                responseContentType = response.ContentType;
                responseBody = streamReader.ReadToEnd();
            }

            return (int)response.StatusCode;
        }
    }
}
