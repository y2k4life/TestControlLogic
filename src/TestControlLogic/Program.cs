using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Minsk.CodeAnalysis;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Syntax;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class SwitchVsIfEnum
    {
        private readonly BoundBinaryExpression _expression;

        [Params(100000, 500000)]
        public int N;

        public SwitchVsIfEnum()
        {
            var op = BoundBinaryOperator.Bind(SyntaxKind.PlusToken, typeof(int), typeof(int));
            _expression = new BoundBinaryExpression(new BoundLiteralExpression(1), op, new BoundLiteralExpression(2));
        }

        [Benchmark]
        public void Switch_Pattern_Matching()
        {
            var varialbes = new Dictionary<VariableSymbol, object>();
            var evaluator = new SwitchPatternEvaluator(_expression, varialbes);

            for (int i = 0; i < N; i++)
            {
                evaluator.Evaluate();
            }
        }

        [Benchmark]
        public void Switch_Enum_Casting()
        {
            var varialbes = new Dictionary<VariableSymbol, object>();
            var evaluator = new SwitchPatternEvaluator(_expression, varialbes);

            for (int i = 0; i < N; i++)
            {
                evaluator.Evaluate();
            }
        }

        [Benchmark]
        public void If_Pattern_Matching()
        {
            var varialbes = new Dictionary<VariableSymbol, object>();
            var evaluator = new IfPatternEvaluator(_expression, varialbes);

            for (int i = 0; i < N; i++)
            {
                evaluator.Evaluate();
            }
        }

        [Benchmark]
        public void If_Enum_Casting()
        {
            var varialbes = new Dictionary<VariableSymbol, object>();
            var evaluator = new IfEnumEvaluator(_expression, varialbes);

            for (int i = 0; i < N; i++)
            {
                evaluator.Evaluate();
            }
        }

        public void If_Using_As()
        {
            var varialbes = new Dictionary<VariableSymbol, object>();
            var evaluator = new NestedIfCastingEvaluator(_expression, varialbes);

            for (int i = 0; i < N; i++)
            {
                evaluator.Evaluate();
            }
        }

        public void Try_Catch_Casting()
        {
            var varialbes = new Dictionary<VariableSymbol, object>();
            var evaluator = new CastingEvaluator(_expression, varialbes);

            for (int i = 0; i < N; i++)
            {
                evaluator.Evaluate();
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {
            var summary = BenchmarkRunner.Run<SwitchVsIfEnum>();
        }
    }
}

