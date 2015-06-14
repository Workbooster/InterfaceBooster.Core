using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InterfaceBooster.SyneryLanguage.Interpretation.General;
using InterfaceBooster.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.ErrorHandling;
using InterfaceBooster.Common.Interfaces.SyneryLanguage;
using InterfaceBooster.Common.Interfaces.SyneryLanguage.Model.Context;
using InterfaceBooster.Common.Interfaces.Utilities.SyneryLanguage;

namespace InterfaceBooster.SyneryLanguage.Interpretation.BaseLanguage.Expressions
{
    /// <summary>
    /// Interprets an expression inside of the general Synery code.
    /// For expressions inside of request statements RequestExpressionInterpreter is used.
    /// </summary>
    public class ExpressionInterpreter : IInterpreter<SyneryParser.ExpressionContext, IValue>
    {
        #region PROPERTIES

        public ISyneryMemory Memory { get; set; }
        public IInterpretationController Controller { get; set; }

        #endregion

        #region PUBLIC METHODS

        public IValue RunWithResult(SyneryParser.ExpressionContext context)
        {
            return InterpretExpression(context);
        }

        #endregion

        #region INTERNAL METHODS

        private IValue InterpretExpression(SyneryParser.ExpressionContext context)
        {
            if (context.ChildCount > 1)
            {
                if (context.children[0] is SyneryParser.ExpressionContext
                    && context.children[2] is SyneryParser.ExpressionContext)
                {
                    // it's a binary operation with two expressions and an operand in between

                    IValue left = InterpretExpression((SyneryParser.ExpressionContext)context.children[0]);
                    IValue right = InterpretExpression((SyneryParser.ExpressionContext)context.children[2]);

                    // logical expressions

                    if (context.OR() != null)
                        return Or(context, left, right);
                    if (context.AND() != null)
                        return And(context, left, right);

                    // equality expressions

                    if (context.EQUAL() != null)
                        return Equal(left, right);
                    if (context.NOTEQUAL() != null)
                        return NotEqual(left, right);

                    // the following expressions need some default values for NULL values
                    // so try to replace NULL values
                    left = GetDefaultForNullValue(left, right);
                    right = GetDefaultForNullValue(right, left);

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
                }

                if (context.QUESTION() != null && context.COLON() != null)
                {
                    // ternary conditional expression
                    // Example: INT test = "A" == "B" ? 7 : 15;

                    IValue conditionValue = InterpretExpression((SyneryParser.ExpressionContext)context.children[0]);
                    IValue trueValue = InterpretExpression((SyneryParser.ExpressionContext)context.children[2]);
                    IValue falseValue = InterpretExpression((SyneryParser.ExpressionContext)context.children[4]);

                    return IfThenElse(context, conditionValue, trueValue, falseValue);
                }
            }
            else if (context.primary() != null)
            {
                // it is a primary expression like a literal or a variable name

                return Controller.Interpret<SyneryParser.PrimaryContext, IValue>(context.primary());
            }
            else if (context.castExpression() != null)
            {
                // it's a cast expression

                return InterpretCastExpression(context.castExpression());
            }

            throw new SyneryInterpretationException(context, string.Format("Unknown expression: '{0}'", context.GetText()));
        }

        #region OPERATIONS

        /// <summary>
        /// Interpret a ternary conditional expression. Example: INT test = "A" == "B" ? 7 : 15;
        /// </summary>
        /// <param name="conditionValue"></param>
        /// <param name="trueValue"></param>
        /// <param name="falseValue"></param>
        /// <returns></returns>
        private IValue IfThenElse(SyneryParser.ExpressionContext context, IValue conditionValue, IValue trueValue, IValue falseValue)
        {
            if (conditionValue.Type != typeof(bool))
                throw new SyneryInterpretationException(context, string.Format("The conditional part of the ternary conditional expression is not a boolean value: {0}", conditionValue.Value));

            if (trueValue.Type != falseValue.Type)
                throw new SyneryInterpretationException(context, string.Format("A ternary conditional operation cannot be applied to two different types '{0}' and '{1}'.", trueValue.Type.PublicName, falseValue.Type.PublicName));

            if ((bool)conditionValue.Value)
            {
                return trueValue;
            }
            else
            {
                return falseValue;
            }
        }

        private IValue InterpretCastExpression(SyneryParser.CastExpressionContext context)
        {
            IValue left = InterpretExpression(context.expression());
            SyneryType conversionType = Controller.Interpret<SyneryParser.PrimitiveTypeContext, SyneryType>(context.primitiveType());

            if (left.Value != null)
            {
                // try to cast the given value.

                try
                {
                    object result;
                    result = Convert.ChangeType(left.Value, conversionType.UnterlyingDotNetType);
                    return new TypedValue(conversionType, result);
                }
                catch (Exception)
                {
                    throw new SyneryInterpretationException(context, string.Format("Error with cast expression '{0}'. Cannot cast the value '{1}' of type '{2}' to new type '{3}'.",
                        context.GetText(), left.Value, left.Type.PublicName, conversionType.PublicName));
                }
            }
            else
            {
                // the given value is NULL. 
                // create a new TypedValue with the given type and a NULL value

                return new TypedValue(conversionType, null);
            }
        }

        private IValue Divide(SyneryParser.ExpressionContext context, IValue left, IValue right)
        {
            ValidateArithemticExpressionValues(context, left, right, "/");

            if (right.Value.Equals(0) || right.Value.Equals(0.0) || right.Value.Equals(0.0M))
            {
                throw new SyneryInterpretationException(context, "Division by zero is not allowed.");
            }

            if (left.Type == typeof(int))
                return new TypedValue(TypeHelper.INT_TYPE,(int)left.Value / (int)right.Value);
            if (left.Type == typeof(double))
                return new TypedValue(TypeHelper.DOUBLE_TYPE,(double)left.Value / (double)right.Value);
            if (left.Type == typeof(decimal))
                return new TypedValue(TypeHelper.DECIMAL_TYPE,(decimal)left.Value / (decimal)right.Value);

            throw new SyneryInterpretationException(context, string.Format("Unhandled case in '{0}'.", this.GetType().Name));
        }

        private IValue Multiply(SyneryParser.ExpressionContext context, IValue left, IValue right)
        {
            ValidateArithemticExpressionValues(context, left, right, "*");

            if (left.Type == typeof(int))
                return new TypedValue(TypeHelper.INT_TYPE,(int)left.Value * (int)right.Value);
            if (left.Type == typeof(double))
                return new TypedValue(TypeHelper.DOUBLE_TYPE,(double)left.Value * (double)right.Value);
            if (left.Type == typeof(decimal))
                return new TypedValue(TypeHelper.DECIMAL_TYPE,(decimal)left.Value * (decimal)right.Value);

            throw new SyneryInterpretationException(context, string.Format("Unhandled case in '{0}'.", this.GetType().Name));
        }

        private IValue Subtract(SyneryParser.ExpressionContext context, IValue left, IValue right)
        {
            ValidateArithemticExpressionValues(context, left, right, "-");

            if (left.Type == typeof(int))
                return new TypedValue(TypeHelper.INT_TYPE,(int)left.Value - (int)right.Value);
            if (left.Type == typeof(double))
                return new TypedValue(TypeHelper.DOUBLE_TYPE,(double)left.Value - (double)right.Value);
            if (left.Type == typeof(decimal))
                return new TypedValue(TypeHelper.DECIMAL_TYPE,(decimal)left.Value - (decimal)right.Value);

            throw new SyneryInterpretationException(context, string.Format("Unhandled case in '{0}'.", this.GetType().Name));
        }

        private IValue Add(SyneryParser.ExpressionContext context, IValue left, IValue right)
        {
            // handle string concatenation if two strings are given
            if (left.Type == typeof(string) && right.Type == typeof(string))
                return new TypedValue(TypeHelper.STRING_TYPE,(string)left.Value + (string)right.Value);

            // otherwise it is an arithmetic operation

            ValidateArithemticExpressionValues(context, left, right, "+");

            if (left.Type == typeof(int))
                return new TypedValue(TypeHelper.INT_TYPE,(int)left.Value + (int)right.Value);
            if (left.Type == typeof(double))
                return new TypedValue(TypeHelper.DOUBLE_TYPE,(double)left.Value + (double)right.Value);
            if (left.Type == typeof(decimal))
                return new TypedValue(TypeHelper.DECIMAL_TYPE,(decimal)left.Value + (decimal)right.Value);

            throw new SyneryInterpretationException(context, string.Format("Unhandled case in '{0}'.", this.GetType().Name));
        }

        private IValue LessThanOrEqual(SyneryParser.ExpressionContext context, IValue left, IValue right)
        {
            ValidateCompareExpressionValues(context, left, right, "<=");

            int compareValue = ((IComparable)left.Value).CompareTo(right.Value);
            return new TypedValue(TypeHelper.BOOL_TYPE,compareValue <= 0);
        }

        private IValue GreaterThanOrEqual(SyneryParser.ExpressionContext context, IValue left, IValue right)
        {
            ValidateCompareExpressionValues(context, left, right, ">=");

            int compareValue = ((IComparable)left.Value).CompareTo(right.Value);
            return new TypedValue(TypeHelper.BOOL_TYPE,compareValue >= 0);
        }

        private IValue LessThan(SyneryParser.ExpressionContext context, IValue left, IValue right)
        {
            ValidateCompareExpressionValues(context, left, right, "<");

            int compareValue = ((IComparable)left.Value).CompareTo(right.Value);
            return new TypedValue(TypeHelper.BOOL_TYPE,compareValue < 0);
        }

        private IValue GreaterThan(SyneryParser.ExpressionContext context, IValue left, IValue right)
        {
            ValidateCompareExpressionValues(context, left, right, ">");

            int compareValue = ((IComparable)left.Value).CompareTo(right.Value);
            return new TypedValue(TypeHelper.BOOL_TYPE,compareValue > 0);
        }

        private IValue NotEqual(IValue left, IValue right)
        {
            if (left.Value != null && right.Value != null)
            {
                return new TypedValue(TypeHelper.BOOL_TYPE,!left.Value.Equals(right.Value));
            }
            else if (left.Value == null && right.Value == null)
            {
                return new TypedValue(TypeHelper.BOOL_TYPE,false);
            }
            else
            {
                return new TypedValue(TypeHelper.BOOL_TYPE,true);
            }
        }

        private IValue Equal(IValue left, IValue right)
        {
            if (left.Value != null && right.Value != null)
            {
                return new TypedValue(TypeHelper.BOOL_TYPE,left.Value.Equals(right.Value));
            }
            else if (left.Value == null && right.Value == null)
            {
                return new TypedValue(TypeHelper.BOOL_TYPE,true);
            }
            else
            {
                return new TypedValue(TypeHelper.BOOL_TYPE,false);
            }

        }

        private IValue Or(SyneryParser.ExpressionContext context, IValue left, IValue right)
        {
            ValidateLogicalExpressionValues(context, left, right, "OR");
            return new TypedValue(TypeHelper.BOOL_TYPE,(bool)left.Value || (bool)right.Value);
        }

        private IValue And(SyneryParser.ExpressionContext context, IValue left, IValue right)
        {
            ValidateLogicalExpressionValues(context, left, right, "AND");
            return new TypedValue(TypeHelper.BOOL_TYPE,(bool)left.Value && (bool)right.Value);
        }

        #endregion

        #region HELPER METHODS

        private IValue GetDefaultForNullValue(IValue potentialNullValue, IValue secondValue)
        {
            if (potentialNullValue.Value == null)
            {
                if (potentialNullValue.Type != null)
                {
                    // this value is a NULL value with a specified type (e.g. from a variable assignment)
                    // get the default value for the specified type

                    return LiteralHelper.GetDefaultValue(potentialNullValue.Type);
                }
                else if (secondValue.Type != null)
                {
                    // only this value is a NULL value without a type
                    // get for the NULL value a default value in the type of the second value

                    return LiteralHelper.GetDefaultValue(secondValue.Type);
                }
            }

            return potentialNullValue;
        }

        #endregion

        #region VALIDATION

        /// <summary>
        /// Checks if both values are of the same type and implement the IComparable interface.
        /// Otherwise it throws a SyneryException.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="operatorName"></param>
        private void ValidateCompareExpressionValues(SyneryParser.ExpressionContext context, IValue left, IValue right, string operatorName)
        {
            if (left.Type != right.Type)
            {
                throw new SyneryInterpretationException(context, string.Format(
                    "Cannot compare two different types '{0}' and '{1}' with a '{2}' expression", left.Type.UnterlyingDotNetType.Name, right.Type.UnterlyingDotNetType.Name, operatorName));
            }
            else if (!left.Type.UnterlyingDotNetType.GetInterfaces().Contains(typeof(IComparable)))
            {
                throw new SyneryInterpretationException(context, string.Format(
                    "The left part of the '{0}' expression is not a comparable value: Type='{1}' / value='{2}'", operatorName, left.Type.UnterlyingDotNetType.Name, left.Value));
            }
            else if (!right.Type.UnterlyingDotNetType.GetInterfaces().Contains(typeof(IComparable)))
            {
                throw new SyneryInterpretationException(context, string.Format(
                    "The right part of the '{0}' expression is not a comparable value: Type='{1}' / value='{2}'", operatorName, right.Type.UnterlyingDotNetType.Name, right.Value));
            }
        }

        /// <summary>
        /// Checks if both values are booleans. Otherwise it throws a SyneryException.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="operatorName"></param>
        private void ValidateLogicalExpressionValues(SyneryParser.ExpressionContext context, IValue left, IValue right, string operatorName)
        {
            if (left.Type != typeof(bool))
            {
                throw new SyneryInterpretationException(context, string.Format(
                    "The left part of the {0} expression is not a boolean value: {1}", operatorName, left.Value));
            }
            else if (right.Type != typeof(bool))
            {
                throw new SyneryInterpretationException(context, string.Format(
                    "The right part of the {0} expression is not a boolean value: {1}", operatorName, right.Value));
            }
        }

        /// <summary>
        /// Checks if the type of both values are equal and if they are int, double or decimal values.
        /// Otherwise it throws a SyneryException
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <param name="operatorName"></param>
        private void ValidateArithemticExpressionValues(SyneryParser.ExpressionContext context, IValue left, IValue right, string operatorName)
        {
            if (left.Type == right.Type)
            {
                if (left.Type == typeof(int) || left.Type == typeof(double) || left.Type == typeof(decimal))
                {
                    return;
                }

                throw new SyneryInterpretationException(context, string.Format(
                    "Cannot apply a '{0}' operation to two values of type '{1}'.", operatorName, left.Type.UnterlyingDotNetType.Name));
            }
            else
            {
                throw new SyneryInterpretationException(context, string.Format(
                    "A '{2}' operation cannot be applied to two different types '{0}' and '{1}'.", left.Type.UnterlyingDotNetType.Name, right.Type.UnterlyingDotNetType.Name, operatorName));
            }
        }

        #endregion

        #endregion
    }
}
