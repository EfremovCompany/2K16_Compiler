using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalyser
{
    class Visitor
    {
        static public string visit(Prgm prgm)
        {
            return Constants.PROGRAM_STATMENT;
        }

        static public string visit(Block block)
        {
            return Constants.BLOCK_STATMENT;
        }
        static public string visit(VaribleDeclarationPart block)
        {
            return Constants.VARIBLE_DECLARATION_RAPT;
        }

        static public string visit(VaribleDeclaration block)
        {
            return Constants.VARIBLE_DECLARATION;
        }

        static public string visit(Type type)
        {
            return Constants.TYPE_STATMENT;
        }

        static public string visit(IntegerType integerType)
        {
            return Constants.INTEGER_TYPE_STATMENT;
        }

        static public string visit(ArrayType arrayType)
        {
            return Constants.ARRAY_TYPE_STATMENT;
        }

        static public string visit(StatmentPart statmentPart)
        {
            return Constants.STATMENT_PART;
        }


        static public string visit(AssignmentStatment assignmentStatment)
        {
            return Constants.ASSIGNMENT_STATMENT;
        }

        static public string visit(VaribleStatment varibleStatment)
        {
            return Constants.VARIBLE_STATMENT;
        }

        static public string visit(ArrayAssignment arrayAssignment)
        {
            return Constants.ARRAY_ASSIGNMENT;
        }

        static public string visit(ReadStatment readStatment)
        {
            return Constants.READ_STATMENT;
        }

        static public string visit(WriteStatment writeStatment)
        {
            return Constants.WRITE_STATMENT;
        }

        static public string visit(Statment writeStatment)
        {
            return Constants.STATMENT;
        }
        static public string visit(IfStatment ifStatment)
        {
            return Constants.IF_STATMENT;
        }
        static public string visit(WhileStatment ifStatment)
        {
            return Constants.WHILE_STATMENT;
        }

        static public string visit(BoolStatment boolStatment)
        {
            return Constants.BOOL_STATMENT;
        }

        static public string visit(BoolExpression boolExpression)
        {
            return Constants.BOOL_EXPRESSION;
        }

        static public string visit(MathStatment mathStatment)
        {
            return Constants.MATH_STATMENT;
        }

        static public string visit(Factor factor)
        {
            return Constants.FACTOR;
        }

        static public string visit(MathExpression mathExpression)
        {
            return Constants.MATH_EXPRESSION;
        }

        static public string visit(RelationalOperator relationalOperator)
        {
            return Constants.RELATIONAL_OPERATOR;
        }

        static public string visit(MathOperator mathOperator)
        {
            return Constants.MATH_OPERATOR;
        }
    }
    
}
