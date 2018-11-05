using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Minsk.CodeAnalysis;
using Minsk.CodeAnalysis.Binding;
using Minsk.CodeAnalysis.Syntax;
using System;
using System.Collections.Generic;

namespace ConsoleApp
{
    public class SwitchVsIfEnum
    {
        private readonly List<BoundExpression> _expressions;

        private readonly VariableSymbol _testVarialbe;

        [Params(100000, 500000)]
        public int N;

        public SwitchVsIfEnum()
        {
            _testVarialbe = new VariableSymbol("a", typeof(int));
            _expressions = new List<BoundExpression>()
            {
                new BoundLiteralExpression(true),
                new BoundVariableExpression(_testVarialbe),
                new BoundAssignmentExpression(new VariableSymbol("b", typeof(int)), new BoundLiteralExpression(1)),
                new BoundBinaryExpression(new BoundLiteralExpression(1), BoundBinaryOperator.Bind(SyntaxKind.PlusToken, typeof(int), typeof(int)), new BoundLiteralExpression(2)),
                new BoundUnaryExpression(BoundUnaryOperator.Bind(SyntaxKind.PlusToken, typeof(int)), new BoundLiteralExpression(1)),
            };
        }

        [Benchmark]
        public void Switch_Pattern_Matching()
        {
            var varialbes = new Dictionary<VariableSymbol, object>();
            var evaluator = new SwitchPatternEvaluator(_expressions[4], varialbes);

            for (int i = 0; i < N; i++)
            {
                evaluator.Evaluate();
            }
        }

        [Benchmark]
        public void Switch_Enum_Casting()
        {
            var varialbes = new Dictionary<VariableSymbol, object>();
            var evaluator = new SwitchEnumEvaluator(_expressions[4], varialbes);

            for (int i = 0; i < N; i++)
            {
                evaluator.Evaluate();
            }
        }

        [Benchmark]
        public void If_Pattern_Matching()
        {
            var varialbes = new Dictionary<VariableSymbol, object>();
            var evaluator = new IfPatternEvaluator(_expressions[4], varialbes);

            for (int i = 0; i < N; i++)
            {
                evaluator.Evaluate();
            }
        }

        [Benchmark]
        public void If_Enum_Casting()
        {
            var varialbes = new Dictionary<VariableSymbol, object>();
            var evaluator = new IfEnumEvaluator(_expressions[4], varialbes);

            for (int i = 0; i < N; i++)
            {
                evaluator.Evaluate();
            }
        }

        [Benchmark]
        public void Random_Switch_Pattern_Matching()
        {
            var varialbes = new Dictionary<VariableSymbol, object>();
            var rnd = new Random(DateTime.Now.Second);

            for (int i = 0; i < N; i++)
            {
                varialbes.Clear();
                varialbes.Add(_testVarialbe, 10);
                var index = rnd.Next(0, 4);
                var evaluator = new SwitchPatternEvaluator(_expressions[index], varialbes);
                evaluator.Evaluate();
            }
        }

        [Benchmark]
        public void Random_Switch_Enum_Casting()
        {
            var varialbes = new Dictionary<VariableSymbol, object>();
            var rnd = new Random(DateTime.Now.Second);

            for (int i = 0; i < N; i++)
            {
                varialbes.Clear();
                varialbes.Add(_testVarialbe, 10);
                var index = rnd.Next(0, 4);
                var evaluator = new SwitchEnumEvaluator(_expressions[index], varialbes);
                evaluator.Evaluate();
            }
        }

        [Benchmark]
        public void Random_If_Pattern_Matching()
        {
            var varialbes = new Dictionary<VariableSymbol, object>();
            var rnd = new Random(DateTime.Now.Second);

            for (int i = 0; i < N; i++)
            {
                varialbes.Clear();
                varialbes.Add(_testVarialbe, 10);
                var index = rnd.Next(0, 4);
                var evaluator = new IfPatternEvaluator(_expressions[index], varialbes);
                evaluator.Evaluate();
            }
        }

        [Benchmark]
        public void Random_If_Enum_Casting()
        {
            var varialbes = new Dictionary<VariableSymbol, object>();
            var rnd = new Random(DateTime.Now.Second);

            for (int i = 0; i < N; i++)
            {
                varialbes.Clear();
                varialbes.Add(_testVarialbe, 10);
                var index = rnd.Next(0, 4);
                var evaluator = new IfEnumEvaluator(_expressions[index], varialbes);
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

