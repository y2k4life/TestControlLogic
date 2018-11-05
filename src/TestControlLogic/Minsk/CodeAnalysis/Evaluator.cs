using Minsk.CodeAnalysis.Binding;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Minsk.CodeAnalysis
{

    internal abstract class Evaluator
    {
        private readonly BoundExpression _root;
        private readonly Dictionary<VariableSymbol, object> _varialbes;

        public Evaluator(BoundExpression root, Dictionary<VariableSymbol, object> varialbes)
        {
            _root = root;
            _varialbes = varialbes;
        }

        public  object Evaluate()
        {
            return EvaluateExpression(_root);
        }

        protected abstract object EvaluateExpression(BoundExpression node);

        protected object EvaluateLiteralExpression(BoundLiteralExpression n)
        {
            return n.Value;
        }

        protected object EvaluateVariableExpression(BoundVariableExpression v)
        {
            return _varialbes.FirstOrDefault(kv => kv.Key.Name == v.Variable.Name).Value;
        }

        protected object EvaluateAssignmentExpression(BoundAssignmentExpression a)
        {
            var value = EvaluateExpression(a.Expression);
            _varialbes[a.Variable] = value;
            return value;
        }

        protected object EvaluateUnaryExpression(BoundUnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);

            switch (u.Op.Kind)
            {
                case BoundUnaryOperatorKind.Identity:
                    return (int)operand;
                case BoundUnaryOperatorKind.Negation:
                    return -(int)operand;
                case BoundUnaryOperatorKind.LogicalNegation:
                    return !(bool)operand;
                default:
                    throw new Exception($"Unexpected unary operator {u.Op}");
            }
        }

        protected object EvaluateBinaryExpression(BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            switch (b.Op.Kind)
            {
                case BoundBinaryOperatorKind.Addition:
                    return (int)left + (int)right;
                case BoundBinaryOperatorKind.Subtraction:
                    return (int)left - (int)right;
                case BoundBinaryOperatorKind.Multiplication:
                    return (int)left * (int)right;
                case BoundBinaryOperatorKind.Division:
                    return (int)left / (int)right;
                case BoundBinaryOperatorKind.LogicAnd:
                    return (bool)left && (bool)right;
                case BoundBinaryOperatorKind.LogicOr:
                    return (bool)left || (bool)right;
                case BoundBinaryOperatorKind.Equals:
                    return Equals(left, right);
                case BoundBinaryOperatorKind.NotEquals:
                    return !Equals(left, right);
                default:
                    throw new Exception($"Unexpected binary operator {b.Op}");
            }
        }
    }
}