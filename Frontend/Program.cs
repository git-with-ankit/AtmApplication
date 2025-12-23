using Frontend.Dependencies;
using System.Threading.Tasks;

namespace Frontend
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var sharedFlow = new SharedFlow();
            await sharedFlow.MainMenu.RunAsync();
        }
    }
}