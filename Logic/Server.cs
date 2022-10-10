using Logic.Networking;

try
{
    var server = new ServerController();
    await server.Listen();
}
catch(Exception ex)
{
    Console.WriteLine(ex);
}