using System.Text.RegularExpressions;
using Framework.Networking.Models.Enums;
using Framework.Networking.Models.Interfaces;

namespace Framework.Networking.Models;

public class HttpResponseHeader : IHttpHeader
{
    public Dictionary<string, string> Headers { get; }

    public HttpStatusCode StatusCode { get; set; }

    public HttpMessageType MessageType { get; set; }

    public string? HttpVersion { get; private set; }

    public HttpResponseHeader(string utf8Data)
    {
        // set message type => needed for further processing
        Headers = new();

        using (var reader = new StringReader(utf8Data))
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

    public HttpResponseHeader(string httpVersion, bool addDefaultHeaders = false)
    {
        Headers = new();
        MessageType = HttpMessageType.Response;
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

                if (parsedString.Count != 3)
                    throw new ArgumentException("Invalid Header");

                if (MessageType != HttpMessageType.Response)
                    throw new ArgumentException("Invalid MessageType", MessageType.ToString());


                // Parse Protocl Version 
                switch (parsedString[0].Split('/', 2)[1])
                {
                    case "1.1":
                        // check if protocol is HTTP
                        if (parsedString[0] != "HTTP")
                            throw new ArgumentException("Invalid Protocol", parsedString[0]);

                        // set version
                        HttpVersion = "1.1";
                        break;
                    default:
                        throw new ArgumentException("Not supported Protocol Version", parsedString[0]);
                        break;
                }

                // Parse Status Code
                if (!Enum.TryParse<HttpStatusCode>(parsedString[1], out var tempStatusCode))
                    throw new ArgumentException("Invalid Status Code");

                StatusCode = tempStatusCode;
                break;
            default:
                parsedString = ParseHeader(line);

                // throw if there are not 2 elements
                if (parsedString.Count != 2)
                    throw new ArgumentException("Invalid Header");

                parsedString[0] = parsedString[0].Trim();
                parsedString[1] = parsedString[1].Trim();

                AddHeader(parsedString[0], parsedString[1]);
                break;
        }
    }

    public void AddHeader(string key, string value)
    {
        Headers.Add(key, value);
    }

    public List<string> ParseHeader(string headerLine)
    {
        return headerLine.Split(':', 2).ToList();
    }

    public override string ToString()
    {
        if (StatusCode == null)
            throw new ArgumentException("Missing StatusCode");

        // write first line
        string headerString = $"HTTP/{HttpVersion} {(int)StatusCode} {StatusCode} \r\n";

        // write all headers
        foreach (var header in Headers)
        {
            headerString += $"{header.Key}: {header.Value}\r\n";
        }

        return headerString;
    }

}