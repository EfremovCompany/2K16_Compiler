using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalyser
{
    class TreeWriter
    {
        public List<string> output = new List<string>();
        string point = ".";
        public void prepareOutputTree(int currentLevel, ITree treeNode)
        {
            foreach (Object element in treeNode.getTokensList())
            {
                try
                {
                    ITree statment = (ITree)element;
                    prepareOutputTree(currentLevel + 1, statment);
                }
                catch
                {
                    String resultString = "";
                    resultString += resultPoint(currentLevel);
                    resultString += ((Token)element).value;
                    output.Add(resultString);
                }
            }
        }

        string resultPoint(int level)
        {
            string resultString = "";
            for (int i = 1; i <= level; ++i)
            {
                resultString += point;
            }
            return resultString;
        }

        public void writeToFile()
        {
            using (StreamWriter writer = new StreamWriter("synt.txt", false))
            {
                foreach (string str in output)
                {
                    writer.WriteLine(str);
                }
            }
        }
    }
}