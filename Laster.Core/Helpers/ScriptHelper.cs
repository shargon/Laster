using Microsoft.CSharp;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Laster.Core.Helpers
{
    public class ScriptHelper
    {
        public class ScriptOptions
        {
            /// <summary>
            /// Include files
            /// </summary>
            public string[] IncludeFiles { get; set; }
            /// <summary>
            /// Extra Usings
            /// </summary>
            public string[] IncludeUsings { get; set; }
            /// <summary>
            /// Inherited 
            /// </summary>
            [Browsable(false)]
            public Type[] Inherited { get; set; }

            public ScriptOptions()
            {
                IncludeFiles = new string[]
                {
                    "system.dll",
                    "system.xml.dll",
                    "system.data.dll",
                    "system.web.dll",
                    "system.windows.forms.dll",
                    "system.drawing.dll",
                };

                IncludeUsings = new string[]
                {
                    "System",
                    "System.Data",
                    "System.Collections.Generic",
                    "System.Drawing.Imaging",
                    "System.IO",
                    "System.Web",
                    "System.Net",
                    "System.Globalization",
                    "System.Windows.Forms",
                    "System.Drawing",
                    "System.Text",
                    "System.Xml",
                    "System.Text.RegularExpressions",
                    "System.Threading"
                };
            }

            public override string ToString()
            {
                return "ScriptOptions";
            }
            /// <summary>
            /// Obtiene el archivo reemplazando los comodines
            /// </summary>
            /// <param name="file">Archivo</param>
            /// <param name="isFromGac">Devuelve si es desde el GAC</param>
            public static string GetFile(string file, out bool isFromGac)
            {
                if (file.StartsWith("@"))
                {
                    file = file.TrimStart('@');
                    file = Path.Combine(Path.GetDirectoryName(Application.ExecutablePath), file);
                    isFromGac = false;
                }
                else isFromGac = true;

                return file;
            }
        }

        /// <summary>
        /// Default Core options
        /// </summary>
        public static ScriptOptions DefaultCoreOptions
        {
            get
            {
                return new ScriptOptions()
                {
                    // Core file
                    IncludeFiles = new string[] { Assembly.GetExecutingAssembly().Location },
                    // Using core
                    //IncludeUsings = new string[] { "" }
                };
            }
        }

        Type _TypeAsm;
        Assembly _Asm;
        string[] _IncludeFiles;
        string[] _IncludeUsings;
        static Dictionary<string, ScriptHelper> _AsmLoaded = new Dictionary<string, ScriptHelper>();

        /// <summary>
        /// Type Asm
        /// </summary>
        public Type Type { get { return _TypeAsm; } }
        /// <summary>
        /// Ensamblado
        /// </summary>
        public Assembly Assembly { get { return _Asm; } }
        /// <summary>
        /// Files
        /// </summary>
        public string[] IncludeFiles { get { return _IncludeFiles; } }
        /// <summary>
        /// Usings
        /// </summary>
        public string[] IncludeUsings { get { return _IncludeUsings; } }

        /// <summary>
        /// Create a Script from File
        /// </summary>
        /// <param name="fileName">Script file</param>
        /// <param name="options">Options</param>
        public static ScriptHelper CreateFromFile(string fileName, ScriptOptions options)
        {
            return CreateFromString(File.ReadAllText(fileName), options);
        }
        /// <summary>
        /// Create a Script from String
        /// </summary>
        /// <param name="codeOrHash">C# Code or hash</param>
        /// <param name="options">Options</param>
        public static ScriptHelper CreateFromString(string codeOrHash, ScriptOptions options)
        {
            if (string.IsNullOrEmpty(codeOrHash)) return null;

            ScriptHelper ret = null;
            if (codeOrHash.Length == 64)
            {
                // Hash
                if (_AsmLoaded.TryGetValue(codeOrHash, out ret)) return ret;
            }

            string hash = HashHelper.HashHex(HashHelper.EHashType.Sha256, Encoding.UTF8, codeOrHash);
            if (_AsmLoaded.TryGetValue(hash, out ret)) return ret;

            List<string> asms = new List<string>();

            // Append files
            if (options != null && options.IncludeFiles != null)
                foreach (string su in options.IncludeFiles)
                {
                    bool isGac;
                    string fp = ScriptOptions.GetFile(su, out isGac);
                    if (!asms.Contains(fp)) asms.Add(fp);
                }

            List<string> usings = new List<string>();

            // Append usings
            if (options != null && options.IncludeUsings != null)
                foreach (string su in options.IncludeUsings)
                    if (!usings.Contains(su)) usings.Add(su);

            string addUsing = "";
            foreach (string su in usings)
                addUsing += "using " + su + ";";

            string herencia = "";
            if (options != null && options.Inherited != null)
            {
                foreach (Type t in options.Inherited)
                {
                    if (t == null) continue;

                    if (herencia != "") herencia += ",";
                    else herencia += ":";

                    herencia += t.FullName.Replace("+", ".");
                }
            }

            codeOrHash = addUsing + " public class Script_" + hash + herencia + "{public Script_" + hash + "(){}" + codeOrHash + "\n}";

            Assembly asm = Compile(codeOrHash, asms.ToArray());
            if (asm == null) return null;

            ret = new ScriptHelper(asm);
            ret._IncludeFiles = asms.ToArray();
            ret._IncludeUsings = usings.ToArray();
            _AsmLoaded.Add(hash, ret);
            return ret;
        }

        ScriptHelper(Assembly asm)
        {
            _Asm = asm;

            //  Search default exported class
            foreach (Type typ in _Asm.GetExportedTypes())
            {
                if (typ.IsNested) continue;
                if (!typ.Attributes.HasFlag(TypeAttributes.Public)) continue;

                if (_TypeAsm != null)
                    throw (new Exception("Multiple public Types exported (Require only one)"));

                _TypeAsm = typ;
            }

            if (_TypeAsm == null)
                throw (new Exception("No public Type exported"));
        }

        /// <summary>
        /// Create Instance
        /// </summary>
        public dynamic CreateNewInstance()
        {
            if (_Asm == null) return null;
            return Activator.CreateInstance(_TypeAsm);
        }
        /// <summary>
        /// Create Instance
        /// </summary>
        public T CreateNewInstance<T>()
        {
            if (_Asm == null) return default(T);
            return (T)Activator.CreateInstance(_TypeAsm);
        }

        #region Compile
        static Assembly Compile(string code, string[] refAsms) { return Compile(code, refAsms, null); }
        static Assembly Compile(string code, string[] refAsms, string fileDest)
        {
            CompilerParameters compilerparams = new CompilerParameters();
            if (fileDest == null)
            {
                compilerparams.GenerateExecutable = false;
                compilerparams.GenerateInMemory = true;
            }
            else
            {
                compilerparams.GenerateExecutable = true;
                compilerparams.GenerateInMemory = false;
                compilerparams.OutputAssembly = fileDest;
            }
            compilerparams.IncludeDebugInformation = false;
            if (refAsms != null) foreach (string rf in refAsms) compilerparams.ReferencedAssemblies.Add(rf);

            CSharpCodeProvider provider = new CSharpCodeProvider();
            CompilerResults results = provider.CompileAssemblyFromSource(compilerparams, code);
            if (results.Errors.HasErrors)
            {
                StringBuilder errors = new StringBuilder("Compilation errors :\r\n");
                foreach (CompilerError error in results.Errors)
                {
                    errors.AppendFormat(" > Line {0},{1}\t: {2}\n", error.Line, error.Column, error.ErrorText);
                }
                throw new Exception(errors.ToString());
            }

            return results.CompiledAssembly;
        }
        #endregion
    }
}