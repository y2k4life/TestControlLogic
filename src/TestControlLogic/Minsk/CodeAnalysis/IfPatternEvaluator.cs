using Minsk.CodeAnalysis.Binding;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis
{
    internal sealed class IfPatternEvaluator : Evaluator
    {
        public IfPatternEvaluator(BoundExpression root, Dictionary<VariableSymbol, object> varialbes)
            : base(root, varialbes)
        {
        }

        protected override object EvaluateExpression(BoundExpression node)
        {
            if (node is BoundLiteralExpression literalExpression)
            {
                return EvaluateLiteralExpression(literalExpression);
            }
            else if (node is BoundVariableExpression variableExpression)
            {
                return EvaluateVariableExpression(variableExpression);
            }
            else if (node is BoundAssignmentExpression assignmentExpression)
            {
                return EvaluateAssignmentExpression((BoundAssignmentExpression)node);
            }
            else if (node is BoundBinaryExpression binaryExpression)
            {
                return EvaluateBinaryExpression(binaryExpression);
            }
            else if (node is BoundUnaryExpression unaryExpressionExpression)
            {
                return EvaluateUnaryExpression(unaryExpressionExpression);
            }

            return null;
        }
    }
}
