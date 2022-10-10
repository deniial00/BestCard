namespace Logic.Networking.Models;

//public enum HTTPProtocl { GET, POST, PUT, DELETE}

public class HttpRequestHeader : IHttpHeader
{
    public Uri Uri { get; private set; }

    public Dictionary<string, string> Headers { get; }

    public HTTPMethodType MethodType { get; private set; }

    public HTTPStatusCode StatusCode { get; private set; }

    public HTTPMessageType MessageType { get; private set; }

    public string HttpVersion { get; private set; }

    public HttpRequestHeader(string utf8Data)
    {
        using (StreamReader reader = new StreamReader(utf8Data))
        {
            string line; int lineNum = 1;

            while ((line = reader.ReadLine()) != null)
            {
                ParseLine(line, lineNum);

                // increment line number var
                lineNum++;
            }
        }
    }

    public HttpRequestHeader(Uri uri, Dictionary<string, string> headers, HTTPMethodType methodType, HTTPStatusCode statusCode, HTTPMessageType messageType, string httpVersion)
    {
        Uri = uri;
        Headers = headers;
        MethodType = methodType;
        StatusCode = statusCode;
        MessageType = messageType;
        HttpVersion = httpVersion;
    }

    private void ParseLine(string line, int lineNum)
    {
        var parsedString = new List<string>();

        switch (lineNum)
        {
            case 1:
                // method + uri + protocol vers
                parsedString = line.Split(' ').ToList();
                if (parsedString.Count != 2)
                {
                    // PROBLEM!!
                }
                if (MessageType == HTTPMessageType.Request)
                {
                    // Parse Method Type
                    try
                    {
                        Enum.TryParse<HTTPMethodType>(parsedString[0], out var tempMethodType);
                        MethodType = tempMethodType;
                    }
                    catch (ArgumentException ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new ArgumentException("Not supported Status Code");
                    }

                    // Parse Url 
                    try
                    {
                        Uri = new Uri(parsedString[1]);
                    }
                    catch (UriFormatException ex)
                    {
                        Console.WriteLine(ex.Message);
                        throw new ArgumentException("Invalid Url");
                    }

                    // Parse Protocol

                }
                else if (MessageType == HTTPMessageType.Response)
                {
                    // Parse Protocol


                    // Parse Status Code
                    Enum.TryParse<HTTPStatusCode>(parsedString[1], out var tempStatusCode);
                    StatusCode = tempStatusCode;
                }
                else
                {
                    throw new ArgumentException("Invalid MessageType");
                }

                // Parse Protocl Version 
                switch (parsedString[2].Split('/')[1])
                {
                    case "1.1":
                        HttpVersion = "1.1";
                        break;
                    default:
                        throw new ArgumentException("Invalid Protocol");
                        break;
                }



                break;
            default:
                parsedString = line.Split(':').ToList();

                // throw if there are not 2 elements
                if (parsedString.Count != 2)
                    throw new ArgumentException("Invalid Header");

                parsedString[0] = parsedString[0].Trim();
                parsedString[1] = parsedString[1].Trim();

                AddHeader(parsedString);
                break;
        }
    }

    public void AddHeader(List<string> Header)
    {
        Headers.Add(Header[0], Header[1]);
    }

    public string ToString()
    {
        string 
        return string;
    }

}