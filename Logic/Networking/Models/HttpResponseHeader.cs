using System.Text.RegularExpressions;

namespace Logic.Networking.Models;

//public enum HTTPProtocl { GET, POST, PUT, DELETE}

public class HttpResponseHeader : IHttpHeader
{
    public Uri Uri { get; private set; }

    public Dictionary<string, string> Headers { get; }

    public HTTPMethodType MethodType { get; private set; }

    public HTTPStatusCode StatusCode { get; private set; }

    public HTTPMessageType MessageType { get; private set; }

    public string HttpVersion { get; private set; }

    public HttpResponseHeader(string utf8Data)
    {
        // set message type => needed for further processing

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

    private void ParseLine(string line, int lineNum)
    {
        var parsedString = new List<string>();

        switch (lineNum)
        {
            case 1:
                // method + uri + protocol vers
                parsedString = line.Split(' ').ToList();
                
                if (parsedString.Count != 2)
                    throw new ArgumentException("Invalid Header");

                if (MessageType != HTTPMessageType.Response)
                    throw new ArgumentException("Invalid MessageType");

                // Parse Protocl Version 
                Regex rg = new Regex(@"\1(HTTP)\2(\/)\3([0-9]\.[0-9])");
                GroupCollection protocolMatchGroup = rg.Match(parsedString[0]).Groups;

                // check if it starts with HTTP and follows with trailing slash => too stupid for regex
                if (protocolMatchGroup[0].Value != "HTTP" || protocolMatchGroup[1].Value != "/")
                    throw new ArgumentException("Invalid Protocol");

                // check supported versions
                switch (protocolMatchGroup[2].Value)
                {
                    case "1.1":
                        HttpVersion = "1.1";
                        break;
                    case "2.0":
                        HttpVersion = "2.0";
                        break;
                    default:
                        throw new ArgumentException("Invalid Protocol");
                }

                // Parse Status Code
                if (!Enum.TryParse<HTTPStatusCode>(parsedString[1], out var tempStatusCode))
                    throw new ArgumentException("Invalid Status Code");

                StatusCode = tempStatusCode;
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

}