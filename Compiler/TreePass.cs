using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxAnalyser
{
    static class ExpressionsList
    {
        static List<Expression> expressions = new List<Expression>();

        static public void AddExpression(Expression ex)
        {
            expressions.Add(ex);
        }

        static public Expression GetElement(int pos)
        {
            return expressions[pos];
        }

        static public void RemoveElement(int pos)
        {
            expressions.RemoveAt(pos);
        }

        static public List<Expression> GetList()
        {
            return expressions;
        }
    }

    static class VarExpressionsList
    {
        static List<ParameterExpression> expressions = new List<ParameterExpression>();

        static public void AddExpression(ParameterExpression ex)
        {
            expressions.Add(ex);
        }

        static public List<ParameterExpression> GetList()
        {
            return expressions;
        }
    }
    class TreePass
    {
        public TreePass(Prgm prgm)
        {
            pass(prgm);
        }

        void pass(Prgm prgm)
        {
            Program.programName = ((Token)prgm.getTokensList()[0]).value;
            Block block = (Block)prgm.getTokensList()[1];
            processVaribleDeclaration((VaribleDeclarationPart)block.getTokensList()[0]);
            processStamentPart((StatmentPart)block.getTokensList()[1]);
        }

        void processVaribleDeclaration(VaribleDeclarationPart varibleDeclarationPart)
        {
            for (int i = 0; i < varibleDeclarationPart.children.Count; i++)
            {
                VaribleDeclaration varibleDeclaration = (VaribleDeclaration)varibleDeclarationPart.getTokensList()[i];
                DefinerProcessor definerProcessor = new DefinerProcessor();
                definerProcessor.process(varibleDeclaration);
            }
            //VaribleDeclaration varibleDeclaration = (VaribleDeclaration)varibleDeclarationPart.getTokensList()[0];
            //DefinerProcessor definerProcessor = new DefinerProcessor();
            //definerProcessor.process(varibleDeclaration);
        }

        void processStamentPart(StatmentPart statmentPart)
        {

            AppDomain domain = AppDomain.CurrentDomain;
            AssemblyName assemblyName = new AssemblyName("test16");
            AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, "./");
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".exe");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Project.Program", TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.BeforeFieldInit);
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("Main", MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig);
            StatmentPartProcessor statmentPartProcessor = new StatmentPartProcessor();
            Statment statment = (Statment)statmentPart.getTokensList()[0];
            foreach (ITree node in statment.getTokensList())
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
            ExpressionsList.AddExpression(Expression.Call(typeof(Console).GetMethod("Read")));
            List<Expression> list = ExpressionsList.GetList();
            BlockExpression blockExpression = Expression.Block(VarExpressionsList.GetList(), ExpressionsList.GetList());
            Expression.Lambda<Action>(blockExpression).CompileToMethod(methodBuilder);
            assemblyBuilder.SetEntryPoint(methodBuilder);
            typeBuilder.CreateType();
            assemblyBuilder.Save(assemblyName.Name + ".exe");
        }
    }
}
