using System.ComponentModel;
using System.Diagnostics;
using Microsoft.JavaScript.NodeApi;
using Microsoft.JavaScript.NodeApi.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TypeScript.Syntax;
using TypeScript.Converter.CSharp;
using Microsoft.CodeAnalysis;
using System;
using TypeScript.Syntax.Converter;

namespace Typescript
{
    [JSExport]
    public partial class Compiler
    {
        // Find the path to the libnode binary for the current platform.
        private static Lazy<NodeEmbeddingThreadRuntime> _nodejsLazy = new Lazy<NodeEmbeddingThreadRuntime>(() =>
            {
                string appDir = Path.GetDirectoryName(typeof(Compiler).Assembly.Location)!;
                string libnodePath = Path.Combine(appDir, "runtimes", "win-x64", "native", "libnode.dll");
                NodeEmbeddingPlatform nodejsPlatform = new(new NodeEmbeddingPlatformSettings { LibNodePath = libnodePath }); //using NodeEmbeddingPlatform nodejsPlatform = new(libnodePath, null);
                NodeEmbeddingThreadRuntime nodejs = nodejsPlatform.CreateThreadRuntime(appDir,
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

                return nodejs;
            });

        public NodeEmbeddingThreadRuntime Node { get => _nodejsLazy.Value; }

        public void OnAst(string path, JSObject result)
        {
        }

        public async Task CompileToCS(string typeScriptFile)
        {
            Console.WriteLine($"Compiling TypeScript file: {typeScriptFile}");

            await Node.RunAsync(async () =>
            {
                // Import typescript compiler
                var typescript = Node.Import("typescript");
                var sourceFile = typescript.CallMethod("createSourceFile", typeScriptFile, File.ReadAllText(typeScriptFile));
                var job = _buildJObject(sourceFile);
                var tsAst = NodeHelper.CreateNode(job);
                var converter = new CSharpConverter(new DefaultConvertContext());
                var csCode = tsAst.ToCSharp();
                Console.WriteLine(csCode);
            });

            Console.WriteLine($"Compilation of {typeScriptFile} completed.");
        }

        // virtual methods for visitor pattern to transfrom Esprima AST to CSHarp roslyn code generation equivelent
        private JObject _buildJObject(JSValue node)
        {
            JObject astNode = new JObject();

            foreach (var property in node.Properties)
            {
                var name = (string)property.Key;
                var value = property.Value;
                if (name == "parent")
                {
                    continue;
                }
                else if (name == "kind")
                {
                    astNode["kind"] = ((NodeKind)(int)value).ToString();
                }
                else
                {
                    if (value.TypeOf() == JSValueType.Null ||
                        value.TypeOf() == JSValueType.Function || 
                        value.TypeOf() == JSValueType.Undefined)
                    {
                        // ignore
                    }
                    else if (value.TypeOf() == JSValueType.String)
                    {
                        astNode[name] = (string)value;
                    }
                    else if (value.TypeOf() == JSValueType.Boolean)
                    {
                        astNode[name] = (bool)value;
                    }
                    else if (value.TypeOf() == JSValueType.Number)
                    {
                        astNode[name] = (float)value;
                    }
                    else if (value.IsArray())
                    {
                        JArray arr = new JArray();
                        foreach (var item in (JSArray)value)
                        {
                            if (value.TypeOf() == JSValueType.String)
                            {
                                arr.Add((string)value);
                            }
                            else if (value.TypeOf() == JSValueType.Boolean)
                            {
                                arr.Add((bool)value);
                            }
                            else if (value.TypeOf() == JSValueType.Number)
                            {
                                arr.Add((float)value);
                            }
                            else if (value.TypeOf() == JSValueType.Object)
                            {
                                arr.Add(_buildJObject(value));
                            }
                        }
                        astNode[name] = arr;
                    }
                    else if (value.TypeOf() == JSValueType.Object)
                    {
                        astNode[name] = _buildJObject(value);
                    }
                    else
                    {
                        throw new Exception("Unexcepted type is found!");
                    }
                }
            }
            return astNode;
        }
    }
}
