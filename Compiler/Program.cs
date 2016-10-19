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
            //--------

            SyntaxAnalyser.Program prog = new SyntaxAnalyser.Program();
            if (prog.IsProgCloseCorrect())
            {
                Console.WriteLine("Program is compiled. Do you want to run it?");
                if (Console.Read() == 'y')
                {
                    Process pro = new Process();
                    pro.StartInfo.UseShellExecute = true;
                    pro.StartInfo.RedirectStandardOutput = false;
                    pro.StartInfo.FileName = SyntaxAnalyser.Program.programName + ".exe";
                    pro.StartInfo.WindowStyle = ProcessWindowStyle.Normal;
                    pro.Start();
                    pro.WaitForExit();
                    return 0;
                }
            }
        
            return -1;
        }
    }
}
