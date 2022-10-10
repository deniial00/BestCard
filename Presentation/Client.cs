using Presentation.Controller;


try 
{
    // wait for server to boot
    Thread.Sleep(500);

    var client = new ClientController();
    Console.Write("Enter username:");
    string username = Console.ReadLine();

    Console.Write("Enter username:");
    string password = Console.ReadLine();

    var res = client.AuthUser(username, password);
    Console.WriteLine(res);
}
catch (IOException ex)
{
    Console.WriteLine(ex.ToString());
}
catch (Exception ex)
{
    Console.WriteLine(ex);
}