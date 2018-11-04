using Minsk.CodeAnalysis.Binding;
using System;
using System.Collections.Generic;
using System.Text;

namespace Minsk.CodeAnalysis
{
    internal sealed class CastingEvaluator : Evaluator
    {
        public CastingEvaluator(BoundExpression root, Dictionary<VariableSymbol, object> varialbes)
            : base(root, varialbes)
        {
        }

        protected override object EvaluateExpression(BoundExpression node)
        {
            try
            {
                BoundLiteralExpression boundLiteralExpression = (BoundLiteralExpression)node;
                return EvaluateLiteralExpression(boundLiteralExpression);
            }
            catch
            {
                try
                {
                    BoundVariableExpression boundVariableExpression = (BoundVariableExpression)node;
                    return EvaluateVariableExpression(boundVariableExpression);
                }
                catch
                {
                    try
                    {
                        BoundAssignmentExpression boundAssignmentExpression = (BoundAssignmentExpression)node;
                        return EvaluateAssignmentExpression(boundAssignmentExpression);
                    }
                    catch
                    {
                        try
                        {
                            BoundUnaryExpression boundUnaryExpression = (BoundUnaryExpression)node;
                            return EvaluateUnaryExpression(boundUnaryExpression);
                        }
                        catch
                        {
                            try
                            {
                                BoundBinaryExpression boundBinaryExpression = (BoundBinaryExpression)node;
                                return EvaluateBinaryExpression(boundBinaryExpression);
                            }
                            catch
                            {
                                return null;
                            }
                        }
                    }
                }
            }
        }
    }
}
