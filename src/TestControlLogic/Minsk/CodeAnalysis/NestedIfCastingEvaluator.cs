using Minsk.CodeAnalysis.Binding;
using System.Collections.Generic;

namespace Minsk.CodeAnalysis
{
    internal sealed class NestedIfCastingEvaluator : Evaluator
    {
        public NestedIfCastingEvaluator(BoundExpression root, Dictionary<VariableSymbol, object> varialbes)
            : base(root, varialbes)
        {
        }

        protected override object EvaluateExpression(BoundExpression node)
        {
            if (node != null)
            {
                BoundLiteralExpression boundLiteralExpression;
                if ((boundLiteralExpression = (node as BoundLiteralExpression)) == null)
                {
                    BoundVariableExpression boundVariableExpression;
                    if ((boundVariableExpression = (node as BoundVariableExpression)) == null)
                    {
                        BoundAssignmentExpression boundAssignmentExpression;
                        if ((boundAssignmentExpression = (node as BoundAssignmentExpression)) == null)
                        {
                            BoundUnaryExpression boundUnaryExpression;
                            if ((boundUnaryExpression = (node as BoundUnaryExpression)) == null)
                            {
                                BoundBinaryExpression boundBinaryExpression;
                                if ((boundBinaryExpression = (node as BoundBinaryExpression)) != null)
                                {
                                    BoundBinaryExpression j = boundBinaryExpression;
                                    return EvaluateBinaryExpression(j);
                                }
                                return null;
                            }
                            return EvaluateUnaryExpression(boundUnaryExpression);
                        }
                        return EvaluateAssignmentExpression(boundAssignmentExpression);
                    }
                    return EvaluateVariableExpression(boundVariableExpression);
                }
                return EvaluateLiteralExpression(boundLiteralExpression);
            }

            return null;
        }
    }
}
