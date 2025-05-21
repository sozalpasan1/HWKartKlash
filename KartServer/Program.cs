// Entry point for the application

using System.Threading.Tasks;

public class Program
{
    public static async Task<int> Main(string[] args)
    {
        // Start the SimpleKartServer
        try {
            await SimpleKartServer.Main(args);
            return 0;
        }
        catch (System.Exception ex) {
            System.Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
}