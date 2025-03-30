global using static CShellNet.Globals;
using CShellNet;
using Typescript;

namespace tsnc
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Echo = false; ;
            await Cmd("npm install typescript@3.8.3").Execute();
            await Cmd("npm install node-api-dotnet").Execute();
            Echo = true;

            Compiler compiler = new Compiler();
            await compiler.CompileToCS(@"s:\github\typescript-converter\demo\input\Greeter.ts");
        }
    }
}
