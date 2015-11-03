using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.QueryLanguage;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.QueryLanguage;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.QueryLanguage.Expressions
{
    /// <summary>
    /// Interprets an expression inside of a request statement.
    /// </summary>
    public class RequestExpressionInterpreter : IInterpreter<SyneryParser.RequestExpressionContext, IExpressionValue, QueryMemory>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IExpressionValue RunWithResult(SyneryParser.RequestExpressionContext context, QueryMemory queryMemory)
        {
            return InterpretRequestExpression(context, queryMemory);
        }

        #endregion

        #region INTERNAL METHODS

        private IExpressionValue InterpretRequestExpression(SyneryParser.RequestExpressionContext context, QueryMemory queryMemory)
        {
            if (context.ChildCount > 1)
            {
                if (context.children[0] is SyneryParser.RequestExpressionContext
                    && context.children[2] is SyneryParser.RequestExpressionContext)
                {
                    // it's a binary operation with two expressions and an operand in between

                    IExpressionValue left = InterpretRequestExpression((SyneryParser.RequestExpressionContext)context.children[0], queryMemory);
                    IExpressionValue right = InterpretRequestExpression((SyneryParser.RequestExpressionContext)context.children[2], queryMemory);

                    // logical expressions

                    if (context.OR() != null)
                        return Or(context, left, right);
                    if (context.AND() != null)
                        return And(context, left, right);

                    // the following expressions need some nullable types
                    // prepare the types for the left and the right expressions

                    left.Expression = PrepareExpressionForBinaryOperation(left, right);
                    right.Expression = PrepareExpressionForBinaryOperation(right, left);

                    // equality expressions

                    if (context.EQUAL() != null)
                        return Equal(left, right);
                    if (context.NOTEQUAL() != null)
                        return NotEqual(left, right);

                    // comparison expressions

                    if (context.GT() != null)
                        return GreaterThan(context, left, right);
                    if (context.LT() != null)
                        return LessThan(context, left, right);
                    if (context.GE() != null)
                        return GreaterThanOrEqual(context, left, right);
                    if (context.LE() != null)
                        return LessThanOrEqual(context, left, right);

                    // arithmetic expressions

                    if (context.PLUS() != null)
                        return Add(context, left, right);
                    if (context.MINUS() != null)
                        return Subtract(context, left, right);
                    if (context.STAR() != null)
                        return Multiply(context, left, right);
                    if (context.SLASH() != null)
                        return Divide(context, left, right);
                    if (context.PERCENT() != null)
                        return Modulo(context, left, right);
                }

                if (context.QUESTION() != null && context.COLON() != null)
                {
                    // ternary conditional expression
                    // Example: INT test = "A" == "B" ? 7 : 15;

                    IExpressionValue conditionValue = InterpretRequestExpression((SyneryParser.RequestExpressionContext)context.children[0], queryMemory);
                    IExpressionValue trueValue = InterpretRequestExpression((SyneryParser.RequestExpressionContext)context.children[2], queryMemory);
                    IExpressionValue falseValue = InterpretRequestExpression((SyneryParser.RequestExpressionContext)context.children[4], queryMemory);

                    // The possible trueValue/falseValue also need to a cast.
                    // Otherwise an Exception "Argument types do not match" is thrown.
                    trueValue.Expression = PrepareExpressionForBinaryOperation(trueValue, falseValue);
                    falseValue.Expression = PrepareExpressionForBinaryOperation(falseValue, trueValue);

                    return IfThenElse(context, conditionValue, trueValue, falseValue);
                }
            }
            else if (context.requestPrimary() != null)
            {
                // it is a primary expression like a literal or a variable name

                return Controller.Interpret<SyneryParser.RequestPrimaryContext, IExpressionValue, QueryMemory>(context.requestPrimary(), queryMemory);
            }
            else if (context.requestCastExpression() != null)
            {
                // it's a cast expression

                return InterpretCastExpression(context.requestCastExpression(), queryMemory);
            }

            throw new SyneryInterpretationException(context, 
                string.Format("Unknown request expression in {0}: '{1}'", this.GetType().Name, context.GetText()));
        }

        private Expression PrepareExpressionForBinaryOperation(IExpressionValue primaryValue, IExpressionValue secondValue)
        {
            Type nullableType;
            Type resultType;

            if (primaryValue.ResultType == null && secondValue.ResultType != null)
            {
                resultType = secondValue.ResultType.UnterlyingDotNetType;
                
                // overwrite the null result type
                primaryValue.ResultType = resultType;
            }
            else
            {
                resultType = primaryValue.ResultType.UnterlyingDotNetType;
            }

            if (resultType == typeof(string))
            {
                // strings already are nullable and don't need to be converted to a nullable type
                nullableType = resultType;
            }
            else
            {
                nullableType = typeof(Nullable<>).MakeGenericType(resultType);
            }

            return Expression.Convert(primaryValue.Expression, nullableType);
        }

        /// <summary>
        /// Interpret a ternary conditional expression. Example: INT test = "A" == "B" ? 7 : 15;
        /// </summary>
        /// <param name="conditionValue"></param>
        /// <param name="trueValue"></param>
        /// <param name="falseValue"></param>
        /// <returns></returns>
        private IExpressionValue IfThenElse(SyneryParser.RequestExpressionContext context, IExpressionValue conditionValue, IExpressionValue trueValue, IExpressionValue falseValue)
        {
            if (conditionValue.ResultType != typeof(bool))
                throw new SyneryInterpretationException(context,
                    string.Format("The conditional part of the ternary conditional expression is not a boolean value. The given type was '{0}'.", conditionValue.ResultType.PublicName));

            if (trueValue.ResultType == null  && falseValue.ResultType == null)
                throw new SyneryInterpretationException(context, "Both value options of the ternary operation have an unknown result type.");

            if (trueValue.ResultType != falseValue.ResultType)
                throw new SyneryInterpretationException(context,
                    string.Format("A ternary conditional operation cannot be applied to two different types '{0}' and '{1}'.",
                    trueValue.ResultType == null ? "NULL" : trueValue.ResultType.PublicName,
                    falseValue.ResultType == null ? "NULL" : falseValue.ResultType.PublicName));

            if (conditionValue.Expression.Type != TypeHelper.BOOL_DOTNET_TYPE)
            {
                // cast the expression to a boolean if the value has a different type
                conditionValue.Expression = Expression.Convert(conditionValue.Expression, TypeHelper.BOOL_DOTNET_TYPE);
            }

            Expression fieldExpression = Expression.Condition(conditionValue.Expression, trueValue.Expression, falseValue.Expression);

            return new ExpressionValue(
                resultType: trueValue.ResultType,
                expression: fieldExpression
            );
        }

        /// <summary>
        /// Creates an Expression that casts a value into an other type.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="queryMemory"></param>
        /// <returns></returns>
        private IExpressionValue InterpretCastExpression(SyneryParser.RequestCastExpressionContext context, QueryMemory queryMemory)
        {
            IExpressionValue left = InterpretRequestExpression(context.requestExpression(), queryMemory);
            SyneryType targetSyneryType = Controller.Interpret<SyneryParser.PrimitiveTypeContext, SyneryType>(context.primitiveType());
            Type targetConversionType = targetSyneryType.UnterlyingDotNetType;

            // create an Expression that calls the Convert.ChangeType(value, type); method
            MethodInfo convetToString = typeof(Convert).GetMethod("ChangeType", new Type[] { typeof(object), typeof(Type) });
            Expression changeTypeExpression = Expression.Call(convetToString, Expression.Convert(left.Expression, typeof(object)), Expression.Constant(targetConversionType));

            Expression fieldExpression = Expression.Convert(changeTypeExpression, targetConversionType);

            return new ExpressionValue(
                resultType: targetSyneryType,
                expression: fieldExpression
            );
        }

        private IExpressionValue Modulo(SyneryParser.RequestExpressionContext context, IExpressionValue left, IExpressionValue right)
        {
            ValidateArithemticExpressionValues(context, left, right, "%");
            
            return new ExpressionValue(
                expression: Expression.Modulo(left.Expression, right.Expression),
                resultType: left.ResultType);
        }

        private IExpressionValue Divide(SyneryParser.RequestExpressionContext context, IExpressionValue left, IExpressionValue right)
        {
            ValidateArithemticExpressionValues(context, left, right, "/");

            return new ExpressionValue(
                expression: Expression.Divide(left.Expression, right.Expression),
                resultType: left.ResultType);
        }

        private IExpressionValue Multiply(SyneryParser.RequestExpressionContext context, IExpressionValue left, IExpressionValue right)
        {
            ValidateArithemticExpressionValues(context, left, right, "*");

            return new ExpressionValue(
                expression: Expression.Multiply(left.Expression, right.Expression),
                resultType: left.ResultType);
        }

        private IExpressionValue Subtract(SyneryParser.RequestExpressionContext context, IExpressionValue left, IExpressionValue right)
        {
            ValidateArithemticExpressionValues(context, left, right, "-");

            return new ExpressionValue(
                expression: Expression.Subtract(left.Expression, right.Expression),
                resultType: left.ResultType);
        }

        private IExpressionValue Add(SyneryParser.RequestExpressionContext context, IExpressionValue left, IExpressionValue right)
        {
            if (left.ResultType == typeof(string) && right.ResultType == typeof(string))
            {
                // handle string concatenation if two strings are given

                MethodInfo concatMethod = typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) });

                return new ExpressionValue(
                    resultType: left.ResultType,
                    expression: Expression.Call(concatMethod, left.Expression, right.Expression)
                );
            }

            ValidateArithemticExpressionValues(context, left, right, "+");

            return new ExpressionValue(
                expression: Expression.Add(left.Expression, right.Expression),
                resultType: left.ResultType);
        }

        private IExpressionValue LessThanOrEqual(SyneryParser.RequestExpressionContext context, IExpressionValue left, IExpressionValue right)
        {
            ValidateCompareExpressionValues(context, left, right, "<=");

            return new ExpressionValue(
                expression: Expression.LessThanOrEqual(left.Expression, right.Expression),
                resultType: TypeHelper.BOOL_TYPE);
        }

        private IExpressionValue GreaterThanOrEqual(SyneryParser.RequestExpressionContext context, IExpressionValue left, IExpressionValue right)
        {
            ValidateCompareExpressionValues(context, left, right, ">=");

            return new ExpressionValue(
                expression: Expression.GreaterThanOrEqual(left.Expression, right.Expression),
                resultType: TypeHelper.BOOL_TYPE);
        }

        private IExpressionValue LessThan(SyneryParser.RequestExpressionContext context, IExpressionValue left, IExpressionValue right)
        {
            ValidateCompareExpressionValues(context, left, right, "<");

            return new ExpressionValue(
                expression: Expression.LessThan(left.Expression, right.Expression),
                resultType: TypeHelper.BOOL_TYPE);
        }

        private IExpressionValue GreaterThan(SyneryParser.RequestExpressionContext context, IExpressionValue left, IExpressionValue right)
        {
            ValidateCompareExpressionValues(context, left, right, ">");

            return new ExpressionValue(
                expression: Expression.GreaterThan(left.Expression, right.Expression),
                resultType: TypeHelper.BOOL_TYPE);
        }

        private IExpressionValue NotEqual(IExpressionValue left, IExpressionValue right)
        {
            return new ExpressionValue(
                expression: Expression.NotEqual(left.Expression, right.Expression),
                resultType: TypeHelper.BOOL_TYPE);
        }

        private IExpressionValue Equal(IExpressionValue left, IExpressionValue right)
        {
            return new ExpressionValue(
                expression: Expression.Equal(left.Expression, right.Expression),
                resultType: TypeHelper.BOOL_TYPE);
        }

        private IExpressionValue Or(SyneryParser.RequestExpressionContext context, IExpressionValue left, IExpressionValue right)
        {
            ValidateLogicalExpressionValues(context, left, right, "OR");

            return new ExpressionValue(
                expression: Expression.OrElse(left.Expression, right.Expression),
                resultType: TypeHelper.BOOL_TYPE);
        }

        private IExpressionValue And(SyneryParser.RequestExpressionContext context, IExpressionValue left, IExpressionValue right)
        {
            ValidateLogicalExpressionValues(context, left, right, "AND");

            return new ExpressionValue(
                expression: Expression.AndAlso(left.Expression, right.Expression),
                resultType: TypeHelper.BOOL_TYPE);
        }

        /// <summary>
        /// Checks if both values are of the same type and implement the IComparable interface.
        /// Otherwise it throws a SyneryException.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="operatorName"></param>
        private void ValidateCompareExpressionValues(SyneryParser.RequestExpressionContext context, IExpressionValue left, IExpressionValue right, string operatorName)
        {
            if (left.ResultType != right.ResultType)
            {
                throw new SyneryInterpretationException(context,
                    string.Format("Cannot compare two different types '{0}' and '{1}' with a '{2}' expression", left.ResultType.PublicName, right.ResultType.PublicName, operatorName));
            }
            else if (!left.ResultType.UnterlyingDotNetType.GetInterfaces().Contains(typeof(IComparable)))
            {
                throw new SyneryInterpretationException(context,
                    string.Format("The left part of the '{0}' expression is not a comparable value: Type='{1}' / value='{2}'", operatorName, left.ResultType.PublicName, left.Expression));
            }
            else if (!right.ResultType.UnterlyingDotNetType.GetInterfaces().Contains(typeof(IComparable)))
            {
                throw new SyneryInterpretationException(context,
                    string.Format("The right part of the '{0}' expression is not a comparable value: Type='{1}' / value='{2}'", operatorName, right.ResultType.PublicName, right.Expression));
            }
        }

        /// <summary>
        /// Checks if both values are booleans. Otherwise it throws a SyneryException.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="operatorName"></param>
        private void ValidateLogicalExpressionValues(SyneryParser.RequestExpressionContext context, IExpressionValue left, IExpressionValue right, string operatorName)
        {
            if (left.ResultType != typeof(bool))
            {
                throw new SyneryInterpretationException(context, 
                    string.Format("The left part of the {0} expression is not a boolean value: {1}", operatorName, left.Expression));
            }
            else if (right.ResultType != typeof(bool))
            {
                throw new SyneryInterpretationException(context, 
                    string.Format("The right part of the {0} expression is not a boolean value: {1}", operatorName, right.Expression));
            }
        }

        /// <summary>
        /// Checks if the type of both values are equal and if they are int, double or decimal values.
        /// Otherwise it throws a SyneryException
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="operatorName"></param>
        private void ValidateArithemticExpressionValues(SyneryParser.RequestExpressionContext context, IExpressionValue left, IExpressionValue right, string operatorName)
        {
            if (left.ResultType == right.ResultType)
            {
                if (left.ResultType == typeof(int) || left.ResultType == typeof(double) || left.ResultType == typeof(decimal))
                {
                    return;
                }

                throw new SyneryInterpretationException(context,
                    string.Format("Cannot apply a '{0}' operation to two values of type '{1}'.", operatorName, left.ResultType.PublicName));
            }
            else
            {
                throw new SyneryInterpretationException(context,
                    string.Format("A '{2}' operation cannot be applied to two different types '{0}' and '{1}'.", left.ResultType.PublicName, right.ResultType.PublicName, operatorName));
            }
        }

        #endregion
    }
}
