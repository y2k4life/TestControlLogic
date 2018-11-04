using Minsk.CodeAnalysis.Binding;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis
{
    internal sealed class SwitchEnumEvaluator : Evaluator
    {
        public SwitchEnumEvaluator(BoundExpression root, Dictionary<VariableSymbol, object> varialbes)
            : base(root, varialbes)
        {
        }

        protected override object EvaluateExpression(BoundExpression node)
        {
            switch (node.Kind)
            {
                case BoundNodeKind.LiteralExpression:
                    return EvaluateLiteralExpression((BoundLiteralExpression)node);
                case BoundNodeKind.VariableExpression:
                    return EvaluateVariableExpression((BoundVariableExpression)node);
                case BoundNodeKind.AssignmentExpression:
                    return EvaluateAssignmentExpression((BoundAssignmentExpression)node);
                case BoundNodeKind.UnaryExpression:
                    return EvaluateUnaryExpression((BoundUnaryExpression)node);
                case BoundNodeKind.BinaryExpression:
                    return EvaluateBinaryExpression((BoundBinaryExpression)node);
                default:
                    return null;
            }
        }
    }
}
