using Framework.Networking.HTTPComponents.Enums;
using Framework.Networking.HTTPComponents.Interfaces;

namespace Framework.Networking.HTTPComponents;

public class HttpRequestHeader : IHttpRequestHeader
{

    public HttpMethodType MethodType { get; private set; }
    
    public string HttpVersion { get; private set; }
    
    public Dictionary<string, string> Headers { get; }
    
    public string Uri { get; private set; }

    public HttpRequestHeader(string utf8Data)
    {
        Headers = new();
        HttpVersion = "";
        Uri = "";

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

    public HttpRequestHeader(HttpMethodType methodType, string uri, string httpVersion = "1.1")
    {
        Headers = new();
        Uri = uri;
        MethodType = methodType;
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

                // Parse Method Type
                try
                {
                    Enum.TryParse<HttpMethodType>(parsedString[0], out var tempMethodType);
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
                    Uri = parsedString[1];
                }
                catch (UriFormatException ex)
                {
                    Console.WriteLine(ex.Message);
                    throw new ArgumentException("Invalid Url");
                }

                var protocolString = parsedString[2].Split('/', 2);

                // Parse Protocl Version 
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

    public static List<string> ParseHeader(string headerLine)
    {
        return headerLine.Split(':', 2).ToList();
    }

    public override string ToString()
    {
        // write first line
        string headerString = $"HTTP/{MethodType} {Uri} HTTP/{HttpVersion} \r\n";

        // write all headers
        foreach (var header in Headers)
        {
            headerString += $"{header.Key}: {header.Value}\r\n";
        }

        return headerString;
    }

}