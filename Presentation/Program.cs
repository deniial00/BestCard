using Presentation.Controller;


try 
{
    var client = new HttpClientController();
    Console.Write("Enter message:");
    string message = Console.ReadLine();
    await client.Request(message);

}
catch(Exception ex)
{
    Console.WriteLine(ex);
}