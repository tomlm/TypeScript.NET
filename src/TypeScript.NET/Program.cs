// Find the path to the libnode binary for the current platform.
using Microsoft.JavaScript.NodeApi;
using Microsoft.JavaScript.NodeApi.Runtime;
using System.Diagnostics;

string appDir = Path.GetDirectoryName(typeof(Program).Assembly.Location)!;
string libnodePath = Path.Combine(appDir, "libnode.dll");
NodeEmbeddingPlatform nodejsPlatform = new(new NodeEmbeddingPlatformSettings { LibNodePath = libnodePath }); //using NodeEmbeddingPlatform nodejsPlatform = new(libnodePath, null);
using NodeEmbeddingThreadRuntime nodejs = nodejsPlatform.CreateThreadRuntime(appDir,
    new NodeEmbeddingRuntimeSettings
    {
        MainScript =
            "globalThis.require = require('module').createRequire(process.execPath);\n"
    });
if (Debugger.IsAttached)
{
    int pid = Process.GetCurrentProcess().Id;
    Uri inspectionUri = nodejs.StartInspector();
    Debug.WriteLine($"Node.js ({pid}) inspector listening at {inspectionUri.AbsoluteUri}");
}
// https://www.linkedin.com/pulse/bundling-nodemodules-your-nodejs-app-one-single-file-xuan-son-nguyen/
// npm install -g browserify
// npm install -g terser
// browserify --node --ignore-missing index.js | terser > bundle.js
var astBuilder = File.ReadAllText(@"s:\scratch\Typescript.NET\src\TypeScript.AstBuilder\_app.js");
var sourcePath = @"S:\github\typescript-converter\demo\input";
var targetPath = @"S:\github\typescript-converter\demo\output";
await nodejs.RunAsync(() =>
{
    var result = JSValue.RunScript($"""
        {astBuilder}
        var project = new TsAstBuilderProject('$"{sourcePath.Replace('\\','/')}', '{targetPath.Replace("\\","/")}');
        project.run();
        """);
    return Task.CompletedTask;
});

Console.WriteLine("Whee!");

