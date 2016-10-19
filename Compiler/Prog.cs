using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalyser
{
    class Program
    {
        static public List<object> identifiers = new List<object>();
        static public string programName = "";
        static public string varibleName = "";
        static public bool isArrayElementLeft = false;

        //
        static bool isCorrect = true;

        public bool IsProgCloseCorrect()
        {
            return isCorrect;
        }

        public Program()
        {
            TokensList tokensList = new TokensList("Output.txt");
            try
            {
                Prgm prgm = new Prgm(tokensList);
                TreePass treePass = new TreePass(prgm);

            }
            catch (Exception e)
            {
                Console.Write(e);
                isCorrect = false;
            }
        }
    }

    class Varible
    {
        public String _name = "";
        public bool _isInit = false;
        public String _type = "";
        public int _length = 0;

        public Varible(String name, string type)
        {
            _name = name;
            _type = type;
        }

        public Varible(String name, string type, int length)
        {
            _name = name;
            _type = type;
            _length = length;
        }

        public void init()
        {
            _isInit = true;
        }

        public string getName()
        {
            return _type;
        }

        public string getType()
        {
            return _type;
        }


        public int getLength()
        {
            return _length;
        }
    }

    class TokensList
    {
        private List<Token> tokensList = new List<Token>();
        public TokensList(string pathToFile)
        {
            ReadTokens(pathToFile);
        }

        public TokensList(TokensList tokens)
        {
            tokensList = new List<Token>(tokens.GetTokens());
        }

        public Token GetToken()
        {
            Token token = tokensList.First();
            tokensList.RemoveAt(0);
            return token;
        }

        public List<Token> GetTokens()
        {
            return tokensList;
        }

        public Token SeeToken()
        {
            return tokensList.First();
        }

        private void ReadTokens(string pathToFile)
        {
            StreamReader sr = new StreamReader(pathToFile);
            Token token = new Token();
            while (!sr.EndOfStream)
            {
                string line = sr.ReadLine();
                string[] parsedLine = line.Split(' ');
                token = new Token();
                token.lineNo = Convert.ToInt32(parsedLine[0]);
                token.value = parsedLine[2];
                token.kind = parsedLine[1];
                tokensList.Add(token);
            }
        }
    }

    struct Token
    {
        public int lineNo;
        public string kind;
        public string value;
    }

    class MathElement
    {
        Token token;
        int priority;
        public MathElement(Token token, int priority)
        {
            this.token = token;
            this.priority = priority;
        }

    }

    /*
    static class Calculator
    {
        static private List<MathElement> mathExpression = new List<MathElement>();
        public static Token calculate()
        {

            
            Token token; token.kind = Constants.CONST_INT; token.lineNo = 0; token.value = result.ToString();
            return token;
        }

        public static int getPriority(Token token)
        {
            if (token.kind == Constants.CONST_INT) return 0;
            else if (token.kind == Constants.PLUS || token.kind == Constants.MINUS) return 1;
            else return 2;
        }

        public static void add(MathElement mathElement)
        {
            mathExpression.Add(mathElement);
        }

        public static void clear()
        {
            mathExpression.Clear();
        }
    }
    */


}


