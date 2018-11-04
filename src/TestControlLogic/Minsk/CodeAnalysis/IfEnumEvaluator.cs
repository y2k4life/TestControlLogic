using Minsk.CodeAnalysis.Binding;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis
{
    internal sealed class IfEnumEvaluator : Evaluator
    {
        public IfEnumEvaluator(BoundExpression root, Dictionary<VariableSymbol, object> varialbes)
            : base(root, varialbes)
        {
        }

        protected override object EvaluateExpression(BoundExpression node)
        {
            if (node.Kind == BoundNodeKind.LiteralExpression)
            {
                return EvaluateLiteralExpression((BoundLiteralExpression)node);
            }
            else if (node.Kind == BoundNodeKind.VariableExpression)
            {
                return EvaluateVariableExpression((BoundVariableExpression)node);
            }
            else if (node.Kind == BoundNodeKind.AssignmentExpression)
            {
                return EvaluateAssignmentExpression((BoundAssignmentExpression)node);
            }
            else if (node.Kind == BoundNodeKind.UnaryExpression)
            {
                return EvaluateUnaryExpression((BoundUnaryExpression)node);
            }
            else if (node.Kind == BoundNodeKind.BinaryExpression)
            {
                return EvaluateBinaryExpression((BoundBinaryExpression)node);
            }

            return null;
        }
    }
}
