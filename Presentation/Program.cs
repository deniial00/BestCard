using Presentation.Controller;


try 
{
    var client = new HttpClientController();
    await client.Request();

}
catch(Exception ex)
{
    Console.WriteLine(ex);
}