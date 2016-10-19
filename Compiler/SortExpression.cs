using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalyser
{
    class SortedExpression
    {
        public object _left;
        public object _right;
        public object _operand;
        

        public SortedExpression(object left, object right, object operand)
        {
            _left = left;
            _right = right;
            _operand = operand;
        }
    }

    class Priority
    {
        public object _element = new Token();
        public int _priority = 0;

        public Priority(object element, int priority)
        {
            _element = element;
            _priority = priority;
        }

    }

    class SortExpression
    {

        static private List<Token> expressionSorted = new List<Token>();
        public static void getSortedExpression(List<Token> expression)
        {
            List<Priority> listPriority = new List<Priority>();
            listPriority = addPriority(expression);
            List<Priority> result = pack(listPriority);
        }


        static List<Priority> pack(List<Priority> expression)
        {
            List<object> result = new List<object>();
            int i = 0;
            bool isHighPriority = true;
            while (isHighPriority)
            {
                if (expression[i]._priority == 2)
                {
                    SortedExpression sortedExpression = new SortedExpression(expression[i - 1], expression[i + 1], expression[i]);
                    Priority priority = new Priority(sortedExpression, 0);
                    expression[i - 1] = priority;
                    expression.RemoveAt(i + 1);
                    expression.RemoveAt(i);
                    --i;
                }
                bool haveHighPriority = false;
                foreach (Priority priority in expression)
                {
                    if (priority._priority == 2)
                    {
                        haveHighPriority = true;
                    }
                }
                isHighPriority = haveHighPriority;
                ++i;
            }
            return expression;
        }

        static List<Priority> addPriority(List<Token> expression)
        {
            List<Priority> listPriority = new List<Priority>();
            foreach (Token token in expression)
            {

                if (token.kind == Constants.PLUS || token.kind == Constants.MINUS)
                {
                    Priority priority = new Priority(token, 1);
                    listPriority.Add(priority);
                }
                else if (token.value == "%" || token.value == "*" || token.value == "/")
                {
                    Priority priority = new Priority(token, 2);
                    listPriority.Add(priority);
                }
                else
                {
                    Priority priority = new Priority(token, 0);
                    listPriority.Add(priority);
                }
            }
            return listPriority;
        }
    
        

        static void passTree(List<Priority> expression, List<int> array)
        {
            for (int i = 0; i != expression.Count; ++i)
            {
                try 
                {
                    Token token = (Token)expression[i]._element;
                    //записываешь в массив значение по индексу i
                    array.Add(Int32.Parse(token.value));
                }
                catch
                {
                    SortedExpression sortExpression = (SortedExpression)expression[i]._element; // * или / 
                    runSortExpression(sortExpression, true);
                    //записываешь значение переменной l в массив по индексу i
                }
            }


            //проходишь массив и складываешь и вычитаешь
        }

        static void runSortExpression(SortedExpression sortExpression, bool isLeft)
        {           
            try 
            {
                //не содержит вложенностей
                Token left = (Token)sortExpression._left; //если не имеет вложенностей левый операнд
                Token right = (Token)sortExpression._right;
                Token operand = (Token)sortExpression._operand;
                //сохраняешь в переменную результат вычисления 
                if (isLeft)
                {
                    //l = left * right
                }
                else
                {
                    //r = left + right
                }
            }
            catch
            {
                try //вложенность содержить правый элемент
                {
                    Token left = (Token)sortExpression._left;
                    Token operand = (Token)sortExpression._operand;
                    runSortExpression((SortedExpression)sortExpression._right, false);
                    if (isLeft)
                    {
                        //l = left + r
                    }
                    else
                    {
                        //r = left + r
                    }
                }
                catch
                {
                    try
                    {
                        //левый содержит вложенность
                        Token right = (Token)sortExpression._right;
                        Token operand = (Token)sortExpression._operand;
                        runSortExpression((SortedExpression)sortExpression._left, true);
                        if (isLeft)
                        {
                            //l = l + right
                        }
                        else
                        {
                            //r = l + right
                        }
                    }
                    catch
                    { //оба содержат вложенность
                        runSortExpression((SortedExpression)sortExpression._left, true);
                        runSortExpression((SortedExpression)sortExpression._right, false);
                        Token operand = (Token)sortExpression._operand;
                        if (isLeft)
                        {
                            //l = l + r
                        }
                        else
                        {
                            //r = l + r
                        }
                    }
                }

            }
        }

    }
}
