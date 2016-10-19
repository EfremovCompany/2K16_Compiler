using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalyser
{
    class DefinerProcessor//объявление пермеременный в var
    {
        Token identifier = new Token();
        Token _type = new Token();
        Token _length = new Token();
        bool _isArray = false;

        public void process(VaribleDeclaration varibleDeclaration)
        {
            identifier = (Token)varibleDeclaration.getTokensList()[0]; //рассказать про косяк сереге
            Type type = (Type)varibleDeclaration.getTokensList()[1];
            try //для массива
            {
                ArrayType arrayType = (ArrayType)type.getTokensList()[0];
                _isArray = true;
                _type = (Token)arrayType.getTokensList()[0];
                _length = (Token)arrayType.getTokensList()[1];
                SemanticAnalizer.checkIsDefineAgain(identifier.value);
                SemanticAnalizer.checkInitEmptyArray(identifier.value, Int32.Parse(_length.value));
                SemanticAnalizer.addVarible(identifier.value, _type.value, Int32.Parse(_length.value));
            }
            catch  // для инта
            {
                IntegerType integerType = (IntegerType)type.getTokensList()[0];
                _isArray = false;
                _type = (Token)integerType.getTokensList()[0];
                SemanticAnalizer.checkIsDefineAgain(identifier.value);
                SemanticAnalizer.addVarible(identifier.value, _type.value);
            }

            generate();
        }

        void generate()//генерируй здесь код
        {
            ParameterExpression ex;
            if (_isArray)
            {
                ex = Expression.Variable(typeof(int[]), identifier.value);
                VarExpressionsList.AddExpression(ex);
                ExpressionsList.AddExpression(ex);
                //добавить ренжу
                ExpressionsList.AddExpression(Expression.Assign(ex, Expression.NewArrayBounds(typeof(int), Expression.Constant(Int32.Parse(_length.value)))));
            }
            else
            {
                ex = Expression.Variable(typeof(int), identifier.value);
                VarExpressionsList.AddExpression(ex);
                ExpressionsList.AddExpression(ex);
            }
        }
    }

    class AssignmentProcessor//Присвоение значения
    {
        bool _isArrayElementLeft = false;
        bool _isMathRight = false; //
        Token _leftOp = new Token();
        Token _elementIndex = new Token();
        List<Token> _rightOp = new List<Token>();
        public void process(AssignmentStatment assignmentStatment)
        {
            VaribleStatment varibleStatment = (VaribleStatment)assignmentStatment.getTokensList()[0];

            if (varibleStatment.getTokensList().Count == 1) // для переменной
            {
                Program.isArrayElementLeft = false;
                _isArrayElementLeft = false;
                _leftOp = (Token)varibleStatment.getTokensList()[0];
                Program.varibleName = _leftOp.value;
                SemanticAnalizer.checkIsDefine(_leftOp.value);
                SemanticAnalizer.initVarible(_leftOp.value);

                _rightOp = getRightOp(assignmentStatment.getTokensList()[1]);
            }
            else //элемент массива
            {
                Program.isArrayElementLeft = true;
                _isArrayElementLeft = true;
                _leftOp = (Token)varibleStatment.getTokensList()[0];
                Program.varibleName = _leftOp.value;
                _elementIndex = (Token)varibleStatment.getTokensList()[2];
                _rightOp = getRightOp(assignmentStatment.getTokensList()[1]);
                SemanticAnalizer.checkArrayOnOutOfRange(_rightOp);
            }
            SemanticAnalizer.checkDivByZero(_rightOp);
            SemanticAnalizer.checkArrayOnOutOfRange(_rightOp);
            generate();

        }

        List<Token> getRightOp(object statment)
        {
            try //для инициализации массива
            {
                ArrayAssignment arrayAssignment = (ArrayAssignment)statment;
                _isMathRight = false;
                return getArrayInitializtionList(arrayAssignment);
            }
            catch //для математического выражения
            {
                MathStatment mathStatment = (MathStatment)statment;
                _isMathRight = true;
                return getMathExpression(mathStatment);
            }
        }

        List<Token> getArrayInitializtionList(ArrayAssignment arrayAssignmnent)
        {
            List<Token> tokens = new List<Token>();
            foreach (Token token in arrayAssignmnent.getTokensList())
            {
                tokens.Add(token);
            }
            SemanticAnalizer.checkVarible(_leftOp.value);
            SemanticAnalizer.checkInitEmptyArray(_leftOp.value, tokens.Count);
            SemanticAnalizer.checkIsLengthArrayEqual(_leftOp.value, tokens.Count);

            if (_isArrayElementLeft)
            {
                SemanticAnalizer.incompatibleTypes();
            }
            else
            {
                SemanticAnalizer.checkCompareTypes(_leftOp.value, Constants.INTARRAY);
            }

            return tokens;
        }

        public static List<Token> getMathExpression(MathStatment mathStatment)
        {
            List<Token> tokens = new List<Token>();
            MathExpression mathExpression = (MathExpression)mathStatment.getTokensList()[0];
            foreach (object item in mathExpression.getTokensList())
            {
                tokens.AddRange(getFactor(item));
            }


            if (!Program.isArrayElementLeft)
            {
                SemanticAnalizer.checkCompareTypes(Program.varibleName, Constants.INT);
            }

            return tokens;
        }

        public static List<Token> getFactor(object item)
        {
            List<Token> tokens = new List<Token>();
            try
            {
                Factor factor = (Factor)item;
                try
                {
                    tokens.Add((Token)factor.getTokensList()[0]);
                }
                catch
                {
                    VaribleStatment varibleStatment = (VaribleStatment)factor.getTokensList()[0];
                    tokens.AddRange(addFactor(varibleStatment));
                }
            }
            catch
            {
                try
                {
                    MathOperator mathOperator = (MathOperator)item;
                    tokens.Add((Token)mathOperator.getTokensList()[0]);
                }
                catch
                {
                    RelationalOperator relationalOperator = (RelationalOperator)item;
                    tokens.Add((Token)relationalOperator.getTokensList()[0]);
                }
            }
            return tokens;
        }

        public static List<Token> addFactor(VaribleStatment varibleStatment)
        {
            List<Token> tokens = new List<Token>();
            foreach (object element in varibleStatment.getTokensList())
            {
                tokens.Add((Token)element);
            }

            SemanticAnalizer.checkVarible(tokens[0].value);

            if (tokens.Count == 4)
            {
                if (tokens[2].kind == Constants.IDENTIFIER)
                {
                    SemanticAnalizer.checkVarible(tokens[2].value);
                }
                else
                {
                    SemanticAnalizer.checkGetElementByIndex(Program.varibleName, Int32.Parse(tokens[2].value));
                }
            }

            return tokens;
        }

        Expression GetConst(string value)
        {
            List<ParameterExpression> vars = VarExpressionsList.GetList();
            for (int i = 0; i < vars.Count; i++)
            {
                if (value == vars[i].Name)
                {
                    return vars[i];
                }
            }
            return Expression.Constant(Int32.Parse(value));
        }

        void generate()
        {
            List<ParameterExpression> vars = VarExpressionsList.GetList();
            if (!_isArrayElementLeft)
            {
                //Array
                Expression left = GetConst(_leftOp.value);
                for (int i = 0; i < vars.Count; i++)
                {
                    if (_leftOp.value == vars[i].Name)
                    {
                        if (vars[i].Type == typeof(int[]))
                        {
                            for (int j = 0; j < _rightOp.Count; j++)
                            {
                                ExpressionsList.AddExpression(Expression.Assign(Expression.ArrayAccess(vars[i], Expression.Constant(j)), Expression.Constant(Int32.Parse(_rightOp[j].value))));                              
                            }
                            return;
                        }
                        else
                        {
                            Expression result = GetConst(_rightOp[0].value);
                            for (int j = 0; j < _rightOp.Count - 1; j += 2)
                            {
                                Expression rigth = GetConst(_rightOp[j + 2].value);
                                if (_rightOp[j + 1].value == "[")
                                {
                                    result = Expression.ArrayAccess(GetConst(_rightOp[j].value), GetConst(_rightOp[j + 2].value));
                                    j = j + 2;
                                }
                                switch(_rightOp[j + 1].value)
                                {
                                    case "+":
                                        result = Expression.Add(result, GetConst(_rightOp[j + 2].value));
                                        break;
                                    case "-":
                                        result = Expression.Subtract(result, GetConst(_rightOp[j + 2].value));
                                        break;
                                    case "*":
                                        result = Expression.Multiply(result, GetConst(_rightOp[j + 2].value));
                                        break;
                                    case "%":
                                        result = Expression.Modulo(result, GetConst(_rightOp[j + 2].value));
                                        break;
                                    case "/":
                                        result = Expression.Divide(result, GetConst(_rightOp[j + 2].value));
                                        break;
                                }
                            }
                            ExpressionsList.AddExpression(Expression.Assign(vars[i], result));
                            return;
                        }
                    }
                }
            }
            else
            {
                Expression left = Expression.ArrayAccess(GetConst(_leftOp.value), GetConst(_elementIndex.value));
                Expression result = GetConst(_rightOp[0].value);
                for (int j = 0; j < _rightOp.Count - 1; j += 2)
                {
                    Expression rigth = GetConst(_rightOp[j + 2].value);
                    if (_rightOp[j + 1].value == "[")
                    {
                        result = Expression.ArrayAccess(GetConst(_rightOp[j].value), GetConst(_rightOp[j + 2].value));
                        j = j + 2;
                    }
                    switch (_rightOp[j + 1].value)
                    {
                        case "+":
                            result = Expression.Add(result, GetConst(_rightOp[j + 2].value));
                            break;
                        case "-":
                            result = Expression.Subtract(result, GetConst(_rightOp[j + 2].value));
                            break;
                        case "*":
                            result = Expression.Multiply(result, GetConst(_rightOp[j + 2].value));
                            break;
                        case "%":
                            result = Expression.Modulo(result, GetConst(_rightOp[j + 2].value));
                            break;
                        case "/":
                            result = Expression.Divide(result, GetConst(_rightOp[j + 2].value));
                            break;
                    }
                }
                ExpressionsList.AddExpression(Expression.Assign(left, result));
                return;
            }
        }

    }

    class ReaderProcessor //
    {
        Token _identifier = new Token();
        Token _elementIndex = new Token();
        bool _isArray = false;
        public void process(ReadStatment readStatment)
        {
            try
            {
                VaribleStatment varibleStatment = (VaribleStatment)readStatment.getTokensList()[0];
                _isArray = false;
                _identifier = (Token)varibleStatment.getTokensList()[0];
                SemanticAnalizer.checkIsDefine(_identifier.value);
                SemanticAnalizer.initVarible(_identifier.value);
            }
            catch//для массива
            {
                _isArray = true;
                List<Token> tokens = AssignmentProcessor.getMathExpression((MathStatment)readStatment.getTokensList()[0]);
                _identifier = tokens[0];
                if (tokens.Count == 1)
                {
                    if (tokens[0].kind == Constants.CONST_INT)
                    {
                        SemanticAnalizer.readAndWriteToConts();
                    }

                    _isArray = false;
                }
                else if (tokens.Count == 4 && (tokens[1].kind == Constants.BRACKET_L) && (tokens[3].kind == Constants.BRACKET_R))
                {
                    _isArray = true;
                    _elementIndex = tokens[2];
                }
                else
                {
                    SemanticAnalizer.InvalidIdentifier();
                }
            }
            generate();
        }

        void generate()
        {
            List<ParameterExpression> vars = VarExpressionsList.GetList();
            if (_isArray)
            {
                for (int i = 0; i < vars.Count; i++)
                {
                    if (_identifier.value == vars[i].Name)
                    {
                        bool isConst = true;
                        for (int j = 0; j < vars.Count; j++)
                        {
                            if (_elementIndex.value == vars[j].Name)
                            {
                                isConst = false;
                                ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant(vars[i].Name)));
                                ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant("[")));
                                ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(int) }), vars[j]));
                                ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant("]")));
                                Expression constant = Expression.Call(typeof(Console).GetMethod("ReadLine"));
                                constant = Expression.Call(typeof(Int32).GetMethod("Parse", new System.Type[] { typeof(string) }), constant);
                                ExpressionsList.AddExpression(Expression.Assign(Expression.ArrayAccess(vars[i], vars[j]), constant));
                                break;
                            }
                        }
                        if (isConst)
                        {
                            ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant(vars[i].Name)));
                            ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant("[")));
                            ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(int) }), Expression.Constant(Int32.Parse(_elementIndex.value))));
                            ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant("]")));
                            ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant(": ")));
                            Expression constant = Expression.Call(typeof(Console).GetMethod("ReadLine"));
                            constant = Expression.Call(typeof(Int32).GetMethod("Parse", new System.Type[] { typeof(string) }), constant);
                            ExpressionsList.AddExpression(Expression.Assign(Expression.ArrayAccess(vars[i], Expression.Constant(Int32.Parse(_elementIndex.value))), constant));
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < vars.Count; i++)
                {
                    if (_identifier.value == vars[i].Name)
                    {
                        ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant(vars[i].Name)));
                        ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant(": ")));
                        Expression constant = Expression.Call(typeof(Console).GetMethod("ReadLine"));
                        constant = Expression.Call(typeof(Int32).GetMethod("Parse", new System.Type[] { typeof(string) }), constant);
                        ExpressionsList.AddExpression(Expression.Assign(vars[i], constant));
                        return;
                    }
                }
            }
        }

    }

    class WriterProcessor //скинуть
    {
        Token _identifier = new Token();
        Token _elementIndex = new Token();
        bool _isArray = false;

        public void process(WriteStatment writeStatment)
        {
            try
            {
                VaribleStatment varibleStatment = (VaribleStatment)writeStatment.getTokensList()[0];
                _isArray = false;
                _identifier = (Token)varibleStatment.getTokensList()[0];
                SemanticAnalizer.checkVarible(_identifier.value);
            }
            catch//для массива
            {

                List<Token> tokens = AssignmentProcessor.getMathExpression((MathStatment)writeStatment.getTokensList()[0]);
                _identifier = tokens[0];
                if (tokens.Count == 1)
                {
                    if (tokens[0].kind == Constants.CONST_INT)
                    {
                        SemanticAnalizer.readAndWriteToConts();
                    }
                    else
                    {
                        SemanticAnalizer.checkVarible(tokens[0].value);
                    }
                    _isArray = false;
                }
                else if (tokens.Count == 4 && (tokens[1].kind == Constants.BRACKET_L) && (tokens[3].kind == Constants.BRACKET_R))
                {
                    _isArray = true;
                    if (tokens[2].kind == Constants.IDENTIFIER)
                    {
                        SemanticAnalizer.checkVarible(tokens[2].value);
                    }
                    _elementIndex = tokens[2];
                }
                else
                {
                    SemanticAnalizer.InvalidIdentifier();
                }

            }
            generate();
        }

        void generate()
        {
            List<ParameterExpression> vars = new List<ParameterExpression>();
            vars = VarExpressionsList.GetList();
            if (_isArray)
            {
                for (int i = 0; i < vars.Count; i++)
                {
                    if (_identifier.value == vars[i].Name)
                    {
                        bool isConst = true;
                        for (int j = 0; j < vars.Count; j++)
                        {
                            if (_elementIndex.value == vars[j].Name)
                            {
                                isConst = false;
                                ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant(vars[i].Name)));
                                ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant("[")));
                                ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(int) }), vars[j]));
                                ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant("] = ")));
                                ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new System.Type[] { typeof(int) }), Expression.Convert(Expression.ArrayAccess(vars[i], vars[j]), typeof(Int32))));
                                break;
                            }
                        }
                        if (isConst)
                        {
                            ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant(vars[i].Name)));
                            ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant("[")));
                            ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant(_elementIndex.value)));
                            ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(String) }), Expression.Constant("] = ")));
                            ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new System.Type[] { typeof(int) }), Expression.Convert(Expression.ArrayAccess(vars[i], Expression.Constant(Int32.Parse(_elementIndex.value))), typeof(Int32))));
                            break;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < vars.Count; i++)
                {
                    if (_identifier.value == vars[i].Name)
                    {
                        ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(string) }), Expression.Constant(vars[i].Name)));
                        ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("Write", new System.Type[] { typeof(string) }), Expression.Constant(" = ")));
                        ExpressionsList.AddExpression(Expression.Call(null, typeof(Console).GetMethod("WriteLine", new System.Type[] { typeof(int) }), vars[i]));
                        break;
                    }
                }
            }
        }
    }

    class IfStatmentProcessor //скинуть
    {
        List<Token> _leftExpression = new List<Token>();
        List<object> _thenExpression = new List<object>();
        List<object> _elseExpression = new List<object>();
        bool isElseAppear = false;
        public void process(IfStatment ifStatment)
        {
            isElseAppear = false;
            BoolStatment boolStatment = (BoolStatment)ifStatment.getTokensList()[0];
            _leftExpression = getLeftExpression((BoolExpression)boolStatment.getTokensList()[0]);
            if (_leftExpression[0].kind == Constants.IDENTIFIER)
            {
                SemanticAnalizer.checkVarible(_leftExpression[0].value);
            }
            if (_leftExpression[2].kind == Constants.IDENTIFIER)
            {
                SemanticAnalizer.checkVarible(_leftExpression[2].value);
            }
            _thenExpression = getElseAndThenStatments((StatmentPart)ifStatment.getTokensList()[1]);

            if (ifStatment.getTokensList().Count == 3)
            {
                isElseAppear = true;
                _elseExpression = getElseAndThenStatments((StatmentPart)ifStatment.getTokensList()[2]);

            }
            generate();
        }

        public static List<Object> getElseAndThenStatments(StatmentPart statmentPart)
        {
            List<Object> statments = StatmentPartProcessor.getStatments(statmentPart);
            return statments;
        }

        public static List<Token> getLeftExpression(BoolExpression boolExpression)
        {
            List<Token> leftExpression = new List<Token>();
            foreach (Object token in boolExpression.getTokensList())
            {
                leftExpression.AddRange(AssignmentProcessor.getFactor(token));
            }
            return leftExpression;
        }

        Expression GetConst(string value)
        {
            List<ParameterExpression> vars = VarExpressionsList.GetList();
            for (int i = 0; i < vars.Count; i++)
            {
                if (value == vars[i].Name)
                {
                    return vars[i];
                }
            }
            return Expression.Constant(Int32.Parse(value));
        }

            void generate()
            {
            Expression left = generateLeftExpression();
            Expression block = generateRightExpression(_thenExpression);
            if (isElseAppear)
            {
                ExpressionsList.AddExpression(Expression.IfThenElse(left, block, generateRightExpression(_elseExpression)));
            }
            else
            {
                ExpressionsList.AddExpression(Expression.IfThen(left, block));
            }
        }

        Expression getPart(string value)
        {
            List<ParameterExpression> vars = VarExpressionsList.GetList();
            for (int i = 0; i < vars.Count; i++)
            {
                if (value == vars[i].Name)
                {
                    return vars[i];
                }
            }
            return Expression.Constant(Int32.Parse(value));
        }

        Expression generateLeftExpression()
        {
            //на тебе
            Expression left = null;
            Expression left_part = null;
            Expression rigth_part = null;
            string value = _leftExpression[1].value;
            if (_leftExpression[1].value == "[")
            {
                left_part = Expression.ArrayAccess(GetConst(_leftExpression[0].value), GetConst(_leftExpression[2].value));
                value = _leftExpression[4].value;
                if (_leftExpression.Count > 6)
                {
                    rigth_part = Expression.ArrayAccess(GetConst(_leftExpression[5].value), GetConst(_leftExpression[7].value));
                }
                else
                {
                    rigth_part = GetConst(_leftExpression[5].value);
                }
            }
            else
            {
                left_part = GetConst(_leftExpression[0].value);
                if (_leftExpression.Count > 3)
                {
                    rigth_part = Expression.ArrayAccess(GetConst(_leftExpression[2].value), GetConst(_leftExpression[4].value));
                }
                else
                {
                    rigth_part = GetConst(_leftExpression[2].value);
                }
            }
            switch (value)
            {
                case "==":
                    left = Expression.Equal(left_part, rigth_part);
                    break;
                case ">=":
                    left = Expression.GreaterThanOrEqual(left_part, rigth_part);
                    break;
                case "!!":
                    left = Expression.NotEqual(left_part, rigth_part);
                    break;
                case ">":
                    left = Expression.GreaterThan(left_part, rigth_part);
                    break;
                case "<=":
                    left = Expression.LessThanOrEqual(left_part, rigth_part);
                    break;
                case "<":
                    left = Expression.LessThan(left_part, rigth_part);
                    break;
                default:
                    break;
            }
            return left;
        }

        Expression generateRightExpression(List<object> expression)
        {
            int i = ExpressionsList.GetList().Count;
            foreach (ITree node in expression)
            {
                if (node.getMethodName() == Constants.ASSIGNMENT_STATMENT)
                {
                    AssignmentProcessor assignmenterProcessor = new AssignmentProcessor();
                    assignmenterProcessor.process((AssignmentStatment)node);
                }
                else if (node.getMethodName() == Constants.READ_STATMENT)
                {
                    ReaderProcessor readerProcessor = new ReaderProcessor();
                    readerProcessor.process((ReadStatment)node);
                }
                else if (node.getMethodName() == Constants.WRITE_STATMENT)
                {
                    WriterProcessor writeProcessor = new WriterProcessor();
                    writeProcessor.process((WriteStatment)node);
                }
                else if (node.getMethodName() == Constants.IF_STATMENT)
                {
                    IfStatmentProcessor ifStatmentProcessor = new IfStatmentProcessor();
                    ifStatmentProcessor.process((IfStatment)node);
                }
                else if (node.getMethodName() == Constants.WHILE_STATMENT)
                {
                    WhileProcessor whileProcessor = new WhileProcessor();
                    whileProcessor.process((WhileStatment)node);
                }
            }
            List<Expression> list = new List<Expression>() { Expression.Empty()};
            int x = i;
            while (ExpressionsList.GetList().Count != i)
            {
                list.Add(ExpressionsList.GetElement(x));
                ExpressionsList.RemoveElement(x);
            }
            return Expression.Block(list);
        }
    }

    class StatmentPartProcessor
    {
        public List<object> _statments = new List<object>();

        public void process(StatmentPart statmentPart) //Выдернуть кастер
        {
            _statments = getStatments(statmentPart);
            generate();
        }

        public static List<object> getStatments(StatmentPart statmentPart) //Выдернуть кастер
        {
            Statment statment = (Statment)statmentPart.getTokensList()[0];
            List<object> statments = statment.getTokensList();
            return statments;
        }

        void generate()
        {
            foreach (ITree node in _statments)
            {
                if (node.getMethodName() == Constants.ASSIGNMENT_STATMENT)
                {
                    AssignmentProcessor assignmenterProcessor = new AssignmentProcessor();
                    assignmenterProcessor.process((AssignmentStatment)node);
                }
                else if (node.getMethodName() == Constants.READ_STATMENT)
                {
                    ReaderProcessor readerProcessor = new ReaderProcessor();
                    readerProcessor.process((ReadStatment)node);
                }
                else if (node.getMethodName() == Constants.WRITE_STATMENT)
                {
                    WriterProcessor writeProcessor = new WriterProcessor();
                    writeProcessor.process((WriteStatment)node);
                }
                else if (node.getMethodName() == Constants.IF_STATMENT)
                {
                    IfStatmentProcessor ifStatmentProcessor = new IfStatmentProcessor();
                    ifStatmentProcessor.process((IfStatment)node);
                }
                else if (node.getMethodName() == Constants.WHILE_STATMENT)
                {
                    WhileProcessor whileProcessor = new WhileProcessor();
                    whileProcessor.process((WhileStatment)node);
                }
            }
        }
    } //Скинуть

    class WhileProcessor
    {
        List<Token> _leftExpression = new List<Token>();
        List<object> _rightExpression = new List<object>();
        public void process(WhileStatment whileStatment)
        {
            BoolStatment boolSatment = (BoolStatment)whileStatment.getTokensList()[0];
            _leftExpression = IfStatmentProcessor.getLeftExpression((BoolExpression)boolSatment.getTokensList()[0]);
            if (_leftExpression[0].kind == Constants.IDENTIFIER)
            {
                SemanticAnalizer.checkVarible(_leftExpression[0].value);
            }
            if (_leftExpression[2].kind == Constants.IDENTIFIER)
            {
                SemanticAnalizer.checkVarible(_leftExpression[2].value);
            }
            _rightExpression = IfStatmentProcessor.getElseAndThenStatments((StatmentPart)whileStatment.getTokensList()[1]);
            generate();
        }

        Expression getPart(string value)
        {
            List<ParameterExpression> vars = VarExpressionsList.GetList();
            for (int i = 0; i < vars.Count; i++)
            {
                if (value == vars[i].Name)
                {
                    return vars[i];
                }
            }
            return Expression.Constant(Int32.Parse(value));
        }

        void generate()
        {
            Expression left = null;
            switch(_leftExpression[1].value)
            {
                case "==":
                    left = Expression.Equal(getPart(_leftExpression[0].value), getPart(_leftExpression[2].value));
                    break;
                case ">=":
                    left = Expression.GreaterThanOrEqual(getPart(_leftExpression[0].value), getPart(_leftExpression[2].value));
                    break;
                case "!!":
                    left = Expression.NotEqual(getPart(_leftExpression[0].value), getPart(_leftExpression[2].value));
                    break;
                case ">":
                    left = Expression.GreaterThan(getPart(_leftExpression[0].value), getPart(_leftExpression[2].value));
                    break;
                case "<=":
                    left = Expression.LessThanOrEqual(getPart(_leftExpression[0].value), getPart(_leftExpression[2].value));
                    break;
                case "<":
                    left = Expression.LessThan(getPart(_leftExpression[0].value), getPart(_leftExpression[2].value));
                    break;
                default:
                    break;
            }
            generateRightExpression(left);
        }

        void generateRightExpression(Expression left)
        {
            LabelTarget label = Expression.Label(typeof(void));
            int i = ExpressionsList.GetList().Count;
            foreach (ITree node in _rightExpression)
            {
                if (node.getMethodName() == Constants.ASSIGNMENT_STATMENT)
                {
                    AssignmentProcessor assignmenterProcessor = new AssignmentProcessor();
                    assignmenterProcessor.process((AssignmentStatment)node);
                }
                else if (node.getMethodName() == Constants.READ_STATMENT)
                {
                    ReaderProcessor readerProcessor = new ReaderProcessor();
                    readerProcessor.process((ReadStatment)node);
                }
                else if (node.getMethodName() == Constants.WRITE_STATMENT)
                {
                    WriterProcessor writeProcessor = new WriterProcessor();
                    writeProcessor.process((WriteStatment)node);
                }
                else if (node.getMethodName() == Constants.IF_STATMENT)
                {
                    IfStatmentProcessor ifStatmentProcessor = new IfStatmentProcessor();
                    ifStatmentProcessor.process((IfStatment)node);
                }
                else if (node.getMethodName() == Constants.WHILE_STATMENT)
                {
                    WhileProcessor whileProcessor = new WhileProcessor();
                    whileProcessor.process((WhileStatment)node);
                }
            }
            List<Expression> list = new List<Expression>();
            int x = i;
            while(ExpressionsList.GetList().Count != i)
            {
                list.Add(ExpressionsList.GetElement(x));
                ExpressionsList.RemoveElement(x);
            }
            Expression rigth = Expression.Block(list);
            var exitLabel = Expression.Label();

            var block =
            Expression.Loop(
                  Expression.IfThenElse(
                    left,
                    rigth, Expression.Break(label)), label);
            ExpressionsList.AddExpression(block);
        }
    }
}
