using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TypeScript.Syntax.Converter;
using TypeScript.Syntax;

namespace TypeScript.Converter.CSharp
{

    public class DefaultConvertContext : IConvertContext
    {
        public DefaultConvertContext()
        {
            this.Project = null;
            this.Namespace = "";
            this.TypeScriptType = false;
            this.Usings = new List<string>();
            this.QualifiedNames = new List<string>();
            this.ExcludeTypes = new List<string>();
        }
        public IProject Project { get; private set; }
        public string Namespace { get; set; }
        public bool TypeScriptType { get; set; }
        public List<string> Usings { get; set; }
        public List<string> QualifiedNames { get; set; }
        public List<string> ExcludeTypes { get; set; }

        public IOutput GetOutput(Syntax.Document doc)
        {
            return null;
        }

        public string TrimTypeName(string typeName)
        {
            return typeName;
        }
    }
}
