using Typescript;

namespace tsnc
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Compiler compiler = new Compiler();
            compiler.CompileToCS(@"s:\github\typescript-converter\demo\input\Greeter.ts");
        }
    }
}
