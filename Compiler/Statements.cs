using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalyser
{
    interface ITree
    {
        string getMethodName();
        List<object> getTokensList();
    }

    class Prgm : ITree
    {
        public List<object> children = new List<object>();

        public Prgm(TokensList tokensList)
        {
            Token token = tokensList.GetToken();
            if (token.kind != Constants.PROGRAM) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.PROGRAM + " but took  " + token.kind.ToString());

            token = tokensList.GetToken();
            if (token.kind == Constants.IDENTIFIER) children.Add(token);
            else throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.IDENTIFIER + " but took  " + token.kind.ToString());

            children.Add(new Block(tokensList));

            token = tokensList.GetToken();
            if (token.kind != Constants.ENDPROGRAM) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.POINT + " but took  " + token.kind.ToString());

        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }

    }

    class Block : ITree
    {
        public List<object> children = new List<object>();

        public Block(TokensList tokensList)
        {
            children.Add(new VaribleDeclarationPart(tokensList));

            children.Add(new StatmentPart(tokensList));
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }

    }
    class VaribleDeclarationPart : ITree
    {
        public List<object> children = new List<object>();

        public VaribleDeclarationPart(TokensList tokensList)
        {
            Token token = tokensList.GetToken();
            if (token.kind != Constants.VAR) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.VAR + " but took  " + token.kind.ToString());

            while (true)
            {
                token = tokensList.SeeToken();
                if (token.kind == Constants.IDENTIFIER)
                {
                    children.Add(new VaribleDeclaration(tokensList));
                }
                else break;
            }

            token = tokensList.GetToken();
            if (token.kind != Constants.ENDVAR) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.ENDVAR + " but took  " + token.kind.ToString());
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class VaribleDeclaration : ITree
    {
        public List<object> children = new List<object>();

        public VaribleDeclaration(TokensList tokensList)
        {
            Token token = tokensList.GetToken();
            if (token.kind == Constants.IDENTIFIER)
            {
                children.Add(token);
                Program.identifiers.Add(token);
            }
            else throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.IDENTIFIER + " but took  " + token.kind.ToString());

            token = tokensList.GetToken();
            if (token.kind != Constants.COLON) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.COLON + " but took  " + token.kind.ToString());

            children.Add(new Type(tokensList));

            token = tokensList.GetToken();
            if (token.kind != Constants.SEMICOLON) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.SEMICOLON + " but took  " + token.kind.ToString());
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class Type : ITree
    {
        public List<object> children = new List<object>();

        public Type(TokensList tokensList)
        {

            Token token = tokensList.SeeToken();
            if (token.kind == Constants.INTARRAY) children.Add(new ArrayType(tokensList));
            else if (token.kind == Constants.INTEGER) children.Add(new IntegerType(tokensList));
            else throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.INTEGER + " or " + Constants.INTARRAY + " but took  " + token.kind.ToString());
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class IntegerType : ITree
    {
        public List<object> children = new List<object>();

        public IntegerType(TokensList tokensList)
        {
            Token token = tokensList.GetToken();
            if (token.kind == Constants.INTEGER)
            {
                children.Add(token);
                Program.identifiers.Add(token);
            }
            else throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.INTEGER + " but took  " + token.kind.ToString());
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class ArrayType : ITree
    {
        public List<object> children = new List<object>();

        public ArrayType(TokensList tokensList)
        {
            Token token = tokensList.GetToken();
            if (token.kind == Constants.INTARRAY)
            {
                children.Add(token);
                Program.identifiers.Add(token);
            }
            else throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.INTARRAY + " but took  " + token.kind.ToString());

            token = tokensList.GetToken();
            if (token.kind != Constants.BRACKET_L) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.BRACKET_L + " but took  " + token.kind.ToString());

            token = tokensList.GetToken();
            if (token.kind == Constants.CONST_INT)
            {
                children.Add(token);
                Program.identifiers.Add(token);
            }
            else throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.CONST_INT + " but took  " + token.kind.ToString());

            token = tokensList.GetToken();
            if (token.kind != Constants.BRACKET_R) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.BRACKET_R + " but took  " + token.kind.ToString());
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }


    class StatmentPart : ITree
    {
        public List<object> children = new List<object>();

        public StatmentPart(TokensList tokensList)
        {
            Token token = tokensList.GetToken();
            if (token.kind != Constants.BEGIN) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.BEGIN + " but took  " + token.kind.ToString());

            children.Add(new Statment(tokensList));

            token = tokensList.GetToken();
            if (token.kind != Constants.END) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.END + " but took  " + token.kind.ToString());
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class Statment : ITree
    {
        public List<object> children = new List<object>();

        public Statment(TokensList tokensList)
        {
            Token token;

            while (true)
            {
                token = tokensList.SeeToken();
                if (token.kind == Constants.IDENTIFIER) children.Add(new AssignmentStatment(tokensList));
                else if (token.kind == Constants.READ) children.Add(new ReadStatment(tokensList));
                else if (token.kind == Constants.WRITE) children.Add(new WriteStatment(tokensList));
                else if (token.kind == Constants.IF) children.Add(new IfStatment(tokensList));
                else if (token.kind == Constants.WHILE) children.Add(new WhileStatment(tokensList));
                else if (token.kind == Constants.MATH) children.Add(new MathStatment(tokensList));
                else break;

                token = tokensList.GetToken();
                if (token.kind != Constants.SEMICOLON) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.SEMICOLON + " but took  " + token.kind.ToString());
            }
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class AssignmentStatment : ITree
    {
        public List<object> children = new List<object>();

        public AssignmentStatment(TokensList tokensList)
        {
            children.Add(new VaribleStatment(tokensList));

            Token token = tokensList.GetToken();
            if (token.kind != Constants.ASSIGN_OP) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.ASSIGN_OP + " but took  " + token.kind.ToString());

            token = tokensList.SeeToken();
            if (token.kind == Constants.MATH) children.Add(new MathStatment(tokensList));
            else if (token.kind == Constants.BRACKET_L) children.Add(new ArrayAssignment(tokensList));
            else throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.MATH + " or " + Constants.BRACKET_L + " but took  " + token.kind.ToString());
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class ArrayAssignment : ITree
    {
        public List<object> children = new List<object>();

        public ArrayAssignment(TokensList tokensList)
        {
            Token token = tokensList.GetToken();
            if (token.kind != Constants.BRACKET_L) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.BRACKET_L + " but took  " + token.kind.ToString());

            while (true)
            {
                token = tokensList.GetToken();
                if (token.kind == Constants.CONST_INT) children.Add(token);
                else throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.CONST_INT + " but took  " + token.kind.ToString());

                token = tokensList.SeeToken();
                if (token.kind == Constants.COMMA)
                {
                    token = tokensList.GetToken();
                    //children.Add(token);
                }
                else break;
            }

            token = tokensList.GetToken();
            if (token.kind != Constants.BRACKET_R) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.BRACKET_R + " but took  " + token.kind.ToString());
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class ReadStatment : ITree
    {
        public List<object> children = new List<object>();

        public ReadStatment(TokensList tokensList)
        {
            Token token = tokensList.GetToken();
            if (token.kind != Constants.READ) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.READ + " but took  " + token.kind.ToString());

            token = tokensList.GetToken();
            if (token.kind != Constants.STREAM_R) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.STREAM_R + " but took  " + token.kind.ToString());

            children.Add(new VaribleStatment(tokensList));

        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class WriteStatment : ITree
    {
        public List<object> children = new List<object>();

        public WriteStatment(TokensList tokensList)
        {
            Token token = tokensList.GetToken();
            if (token.kind != Constants.WRITE) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.WRITE + " but took  " + token.kind.ToString());

            token = tokensList.GetToken();
            if (token.kind != Constants.STREAM_L) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.STREAM_L + " but took  " + token.kind.ToString());

            token = tokensList.SeeToken();

            if (token.kind == Constants.IDENTIFIER) children.Add(new VaribleStatment(tokensList));
            else if (token.kind == Constants.MATH) children.Add(new MathStatment(tokensList));
            else throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.IDENTIFIER + " or " + Constants.MATH + " but took  " + token.kind.ToString());
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class IfStatment : ITree
    {
        public List<object> children = new List<object>();

        public IfStatment(TokensList tokensList)
        {
            Token token = tokensList.GetToken();
            if (token.kind != Constants.IF) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.IF + " but took  " + token.kind.ToString());

            children.Add(new BoolStatment(tokensList));

            token = tokensList.GetToken();
            if (token.kind != Constants.THEN) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.THEN + " but took  " + token.kind.ToString());

            children.Add(new StatmentPart(tokensList));

            token = tokensList.SeeToken();
            if (token.kind == Constants.ELSE)
            {
                token = tokensList.GetToken();
                //children.Add(token);

                children.Add(new StatmentPart(tokensList));
            }

            token = tokensList.GetToken();
            if (token.kind != Constants.ENDIF) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.ENDIF + " but took  " + token.kind.ToString());

        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class WhileStatment : ITree
    {
        public List<object> children = new List<object>();

        public WhileStatment(TokensList tokensList)
        {
            Token token = tokensList.GetToken();
            if (token.kind != Constants.WHILE) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.WHILE + " but took  " + token.kind.ToString());

            children.Add(new BoolStatment(tokensList));

            children.Add(new StatmentPart(tokensList));

            token = tokensList.GetToken();
            if (token.kind != Constants.ENDWHILE) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.ENDWHILE + " but took  " + token.kind.ToString());

        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class BoolStatment : ITree
    {
        public List<object> children = new List<object>();

        public BoolStatment(TokensList tokensList)
        {
            Token token = tokensList.GetToken();
            if (token.kind != Constants.BOOL) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.BOOL + " but took  " + token.kind.ToString());

            token = tokensList.GetToken();
            if (token.kind != Constants.PARANTHESIS_L) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.PARANTHESIS_L + " but took  " + token.kind.ToString());

            children.Add(new BoolExpression(tokensList));

            token = tokensList.GetToken();
            if (token.kind != Constants.PARANTHESIS_R) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.PARANTHESIS_R + " but took  " + token.kind.ToString());
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class BoolExpression : ITree
    {
        public List<object> children = new List<object>();

        public BoolExpression(TokensList tokensList)
        {
            children.Add(new Factor(tokensList));

            children.Add(new RelationalOperator(tokensList));

            children.Add(new Factor(tokensList));
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class MathStatment : ITree
    {
        public List<object> children = new List<object>();

        public MathStatment(TokensList tokensList)
        {
            Token token = tokensList.GetToken();
            if (token.kind != Constants.MATH) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.MATH + " but took  " + token.kind.ToString());

            token = tokensList.GetToken();
            if (token.kind != Constants.PARANTHESIS_L) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.PARANTHESIS_L + " but took  " + token.kind.ToString());

            children.Add(new MathExpression(tokensList));

            token = tokensList.GetToken();
            if (token.kind != Constants.PARANTHESIS_R) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.PARANTHESIS_R + " but took  " + token.kind.ToString());
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class MathExpression : ITree
    {
        public List<object> children = new List<object>();

        public MathExpression(TokensList tokensList)
        {
            children.Add(new Factor(tokensList));

            while (true)
            {
                Boolean isOperator = false;
                MathOperator mathOperator = new MathOperator(tokensList, ref isOperator);
                if (mathOperator.getTokensList().Count > 0)
                {
                    children.Add(mathOperator);
                }
                if (isOperator) children.Add(new Factor(tokensList));
                else break;
            }
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class Factor : ITree
    {
        public List<object> children = new List<object>();

        public Factor(TokensList tokensList)
        {
            Token token = tokensList.SeeToken();
            if (token.kind == Constants.IDENTIFIER)
            {
                children.Add(new VaribleStatment(tokensList));
            }
            else if (token.kind == Constants.CONST_INT)
            {
                token = tokensList.GetToken();
                children.Add(token);
            }
            else throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.IDENTIFIER + " or " + Constants.CONST_INT + " but took  " + token.kind.ToString());
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class RelationalOperator : ITree
    {
        public List<object> children = new List<object>();

        public RelationalOperator(TokensList tokensList)
        {
            Token token = tokensList.GetToken();
            if (token.kind == Constants.EQ_OP) children.Add(token);
            else if (token.kind == Constants.NE_OP) children.Add(token);
            else if (token.kind == Constants.MORE_OP) children.Add(token);
            else if (token.kind == Constants.LESS_OP) children.Add(token);
            else if (token.kind == Constants.LE_OP) children.Add(token);
            else if (token.kind == Constants.GE_OP) children.Add(token);

            else throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.RELATIONAL_OPERATOR + " but took  " + token.kind.ToString());
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class MathOperator : ITree
    {
        public List<object> children = new List<object>();

        public MathOperator(TokensList tokensList, ref Boolean isOperator)
        {
            Token token = tokensList.SeeToken();
            if (token.kind == Constants.MINUS || token.kind == Constants.PLUS ||
                token.kind == Constants.MULT || token.kind == Constants.DIV || token.kind == Constants.MOD)
            {
                isOperator = true;
                token = tokensList.GetToken();
                children.Add(token);
            }

            else
            {
                if (token.kind == Constants.PARANTHESIS_R) isOperator = false;
                else throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.MATH_OPERATOR + " but took  " + token.kind.ToString());
            }
        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

    class VaribleStatment : ITree
    {
        public List<object> children = new List<object>();

        public VaribleStatment(TokensList tokensList)
        {
            Token token = tokensList.GetToken();
            if (token.kind == Constants.IDENTIFIER) children.Add(token);
            else throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.IDENTIFIER + " but took  " + token.kind.ToString());

            token = tokensList.SeeToken();
            if (token.kind == Constants.BRACKET_L)
            {
                token = tokensList.GetToken();
                if (token.kind != Constants.BRACKET_L) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.BRACKET_L + " but took  " + token.kind.ToString());
                else children.Add(token);
                token = tokensList.GetToken();
                if (token.kind == Constants.IDENTIFIER) children.Add(token);
                else if (token.kind == Constants.CONST_INT) children.Add(token);
                else throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.CONST_INT + " but took  " + token.kind.ToString());

                token = tokensList.GetToken();
                if (token.kind != Constants.BRACKET_R) throw new System.Exception("Line " + token.lineNo.ToString() + " : expected " + Constants.BRACKET_R + " but took  " + token.kind.ToString());
                else children.Add(token);
            }

        }

        public string getMethodName()
        {
            return Visitor.visit(this);
        }

        public List<object> getTokensList()
        {
            return children;
        }
    }

}
