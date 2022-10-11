using Framework.Networking.HTTPComponents.Enums;
using Framework.Networking.HTTPComponents.Interfaces;

namespace Framework.Networking.HTTPComponents;

public class HttpResponseHeader : IHttpResponseHeader
{
    public HttpStatusCode StatusCode { get; set; }
    
    public string HttpVersion { get; private set; }

    public Dictionary<string, string> Headers { get; }

    public HttpResponseHeader(string utf8Data)
    {
        Headers = new();
        HttpVersion = "";
        try
        {
            using (var reader = new StringReader(utf8Data))
            {
                string line;
                int lineNum = 1;

                while ((line = reader.ReadLine()) != null)
                {
                    if (line == "")
                        break;

                    ParseLine(line, lineNum);

                    // increment line number var
                    lineNum++;
                }
            }

        }
        catch (ArgumentException) { throw; }
        catch (NotSupportedException) { throw; }
        catch (Exception){ throw; }
    }

    public HttpResponseHeader(string httpVersion, bool addDefaultHeaders = false)
    {
        Headers = new();
        HttpVersion = httpVersion;
    }

    private void ParseLine(string line, int lineNum)
    {
        var parsedString = new List<string>();

        switch (lineNum)
        {
            case 1:
                // version + statusCode + statusCodeMessage
                parsedString = line.Split(' ').ToList();

                if (parsedString.Count != 3)
                    throw new ArgumentException("Invalid Header", parsedString.ToString());

                // Parse Protocl Version 
                var protocolString = parsedString[0].Split('/', 2);
                switch (protocolString[1])
                {
                    case "1.1":
                        // check if protocol is HTTP
                        if (protocolString[0] != "HTTP")
                            throw new ArgumentException("Invalid Protocol", protocolString[0]);

                        // set version
                        HttpVersion = "1.1";
                        break;
                    default:
                        throw new ArgumentException("Invalid Protocol", protocolString.ToString());
                }

                // Parse Status Code
                if (!Enum.TryParse<HttpStatusCode>(parsedString[1], out var tempStatusCode))
                    throw new ArgumentException("Invalid Status Code", parsedString[1]);

                StatusCode = tempStatusCode;
                break;
            default:
                parsedString = ParseHeader(line);

                // throw if there are not 2 elements
                if (parsedString.Count != 2)
                    throw new ArgumentException("Invalid Header", parsedString.ToString());

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