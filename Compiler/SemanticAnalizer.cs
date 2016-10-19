using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalyser
{
    static class SemanticAnalizer
    {
        static public List<Varible> _varibles = new List<Varible>();
        public static void addVarible(string name, string type) //+
        {
            _varibles.Add(new Varible(name, type));
        }

        public static void addVarible(string name, string type, int length) //+
        {
            _varibles.Add(new Varible(name, type, length));
        }

        public static List<Varible> getVaribles()
        {
            return _varibles;
        }

        public static void initVarible(string name)  //+
        {
            foreach (Varible varible in _varibles)
            {
                if (varible._name == name)
                {
                    varible._isInit = true;
                }
            }
        }

        public static void checkIsDefineAgain(string name) //+
        {
            foreach (Varible varible in _varibles)
            {
                if (varible._name == name)
                {
                    try
                    {
                        throw new System.Exception("Varible " + name + " was define already");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        Environment.Exit(0);
                    }
                    break;
                }
            }
        }



        public static void checkIsInit(string name) //+
        {
            foreach (Varible varible in _varibles)
            {
                if (varible._name == name)
                {
                    if (!varible._isInit)
                    {
                        try
                        {
                            throw new System.Exception("Varible " + name + " is not init");
                        }
                        catch(Exception ex)
                        {
                            Console.WriteLine(ex.Message);
                            Environment.Exit(0);
                        }
                        break;
                    }
                }
            }
        }

        public static void checkIsDefine(string name) //+
        {
            bool isDefine = false;
            foreach (Varible varible in _varibles)
            {
                if (varible._name == name)
                {
                    isDefine = true;
                    break;
                }
            }
            if (!isDefine)
            {
                throw new System.Exception("Varible " + name + " was not define");
            }
        }


        public static void checkDivByZero(List<Token> rightOp)
        {
            Boolean isDivision = false;
            foreach (Token token in rightOp)
            {
                if (isDivision)
                {
                    if (Int32.Parse(token.value) == 0)
                    {
                        throw new System.Exception("Line " + token.lineNo.ToString() + " division by zero");
                    }
                }
                else
                {
                    if (token.value == "/")
                    {
                        isDivision = true;
                    }
                }
            }
        }

        public static void checkInitEmptyArray(string name, int length) //+
        {
            if (length == 0)
            {
                throw new System.Exception("Init empty array "  + name);
            }
        }

        public static void checkIsLengthArrayEqual(string name, int length) //+
        {
            foreach (Varible varible in _varibles)
            {
                if (varible._name == name)
                {
                    if (varible._length != length)
                    {
                        throw new System.Exception("Incorrect length for array " + name + " expext : " + varible._length);
                        break;
                    }
                }
            }
        }

        public static void checkGetElementByIndex(string name, int index) //прочекать
        {
            foreach (Varible varible in _varibles)
            {
                if (varible._name == name)
                {
                    if (index < 0 || index > varible._length)
                    {
                        throw new System.Exception("outside the array length " + name);
                        break;
                    }
                }
            }
        }

        public static void checkArrayOnOutOfRange(List<Token> expression) //+
        {
            for (int i = 0; i != expression.Count; ++i)
            {
                Token token = (Token)expression[i];
                if (token.kind == Constants.BRACKET_L)
                {
                    if (expression[i - 1].kind == Constants.CONST_INT)
                    {
                        checkGetElementByIndex(((Token)expression[i - 1]).value, Int32.Parse(token.value));
                    }
                    
                }
            }
        }

        public static void checkVarible(string name) //+
        {
            checkIsDefine(name);
            checkIsInit(name);
        }

        public static void incompatibleTypes() //+
        {
            //because to array element assign array
            throw new System.Exception("incompatible types ");
        }

        public static void InvalidIdentifier() 
        {
            throw new System.Exception("Invalid identifier ");
        }

        public static void checkCompareTypes(string name, string type) //+
        {
            foreach (Varible varible in _varibles)
            {
                if (varible._name == name)
                {
                    if (varible._type != type.ToLower())
                    {
                        throw new System.Exception("incompatible types ");
                        break;
                    }
                }
            }
        }

        public static void readAndWriteToConts()
        {
            throw new System.Exception("Can not read and write to const ");
        }

    }
}
