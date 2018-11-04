using Minsk.CodeAnalysis.Binding;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis
{
    internal sealed class SwitchPatternEvaluator : Evaluator
    {
        public SwitchPatternEvaluator(BoundExpression root, Dictionary<VariableSymbol, object> varialbes)
            : base(root, varialbes)
        {
        }

        protected override object EvaluateExpression(BoundExpression node)
        {
            switch (node)
            {
                case BoundLiteralExpression literalExpression:
                    return EvaluateLiteralExpression(literalExpression);
                case BoundVariableExpression variableExpression:
                    return EvaluateVariableExpression(variableExpression);
                case BoundAssignmentExpression assignmentExpression:
                    return EvaluateAssignmentExpression(assignmentExpression);
                case BoundUnaryExpression unaryExpressionExpression:
                    return EvaluateUnaryExpression(unaryExpressionExpression);
                case BoundBinaryExpression binaryExpression:
                    return EvaluateBinaryExpression(binaryExpression);
                default:
                    return null;
            }
        }
    }
}
