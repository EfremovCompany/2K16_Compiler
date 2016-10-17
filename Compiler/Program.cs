using SyntaxAnalyser;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    class Program
    {
        static int Main(string[] args)
        {
            //Lexer
            Process proc = new Process();
            proc.StartInfo.UseShellExecute = true;
            proc.StartInfo.RedirectStandardOutput = false;
            proc.StartInfo.FileName = @"LexicalAnalisator.exe";
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
            proc.Start();
            proc.WaitForExit();

            //Sint
            SyntaxAnalyser.Program prog = new SyntaxAnalyser.Program();
            //ITree tree = SyntaxAnalyser.Program.tree;
            //if (!prog.IsSyntaxAnalysIsOK())
             //   return -1;

            //Semantic

            //Code-Generation
            //Generator.Generator gen = new Generator.Generator();
            

            return 0;
        }
    }
}
