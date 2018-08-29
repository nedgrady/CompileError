using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

namespace CompileError
{
    class Program
    {
        [DllImport("Kernel32.dll")]
        public static extern int GetLastError();

        [DllImport("Kernel32.dll")]
        public static extern void SetLastError(uint errorCode);

        const string SOURCE = @"using System;

        public static class Test
        {
            public static void PrintTest()
            {
                Console.WriteLine(""PrintTest()"");
            }
        }";

        static void Main(string[] args)
        {
            CodeDomProvider cpd = new CSharpCodeProvider();

            CompilerParameters cp = new CompilerParameters()
            {
                GenerateExecutable = false,
                GenerateInMemory = true
            };
            cp.ReferencedAssemblies.Add("System.dll");

            Console.WriteLine($"GetLastError Before Compile: {GetLastError()}");
            CompilerResults cr = cpd.CompileAssemblyFromSource(cp, SOURCE);
            Console.WriteLine($"GetLastError After Compile: {GetLastError()}");
            SetLastError(0);

            if (cr.Errors.Count > 0)
            {
                foreach (CompilerError error in cr.Errors)
                {
                    Console.WriteLine(error.ErrorText);
                }
                Console.ReadLine();
                return;
            }

            Console.WriteLine(cr.PathToAssembly ?? "null path");

            cr.CompiledAssembly
                .GetTypes()
                .First(t => t.Name == "Test")
                .GetMethod("PrintTest")
                .Invoke(null, null);
            Console.ReadLine();
        }
    }
}
