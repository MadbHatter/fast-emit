using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace FastEmit
{
    public class Visitor : ExpressionVisitor
    {
        private readonly EmitContext _context;

        public Visitor(EmitContext context)
        {
            _context = context;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Type == typeof(byte))
                _context.Generator.Emit(OpCodes.Ldc_I4_S, Convert.ToByte(node.Value));
            else if (node.Type == typeof(sbyte))
                _context.Generator.Emit(OpCodes.Ldc_I4_S, Convert.ToSByte(node.Value));
            else if (node.Type == typeof(short))
                _context.Generator.Emit(OpCodes.Ldc_I4, Convert.ToInt16(node.Value));
            else if (node.Type == typeof(ushort))
                _context.Generator.Emit(OpCodes.Ldc_I4, Convert.ToUInt16(node.Value));
            else if (node.Type == typeof(int))
                _context.Generator.Emit(OpCodes.Ldc_I4, Convert.ToInt32(node.Value));
            else if (node.Type == typeof(uint))
                _context.Generator.Emit(OpCodes.Ldc_I4, Convert.ToUInt32(node.Value));
            else if (node.Type == typeof(long)) 
                _context.Generator.Emit(OpCodes.Ldc_I8, Convert.ToInt64(node.Value));
            else if (node.Type == typeof(ulong))
                _context.Generator.Emit(OpCodes.Ldc_I8, Convert.ToUInt64(node.Value));
            else if (node.Type == typeof(float))
                _context.Generator.Emit(OpCodes.Ldc_R4, Convert.ToSingle(node.Value));
            else if (node.Type == typeof(double))
                _context.Generator.Emit(OpCodes.Ldc_R8, Convert.ToDouble(node.Value));
            else if (node.Type == typeof(string))
                _context.Generator.Emit(OpCodes.Ldstr, Convert.ToString(node.Value));
            else if (node.Type == typeof(bool))
                _context.Generator.Emit(Convert.ToBoolean(node.Value) ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);

            return node;
        }

        protected override Expression VisitConditional(ConditionalExpression node)
        {
            var falseBegin = _context.Generator.DefineLabel();
            var endAll = _context.Generator.DefineLabel();
            Visit(node.Test);
            _context.Generator.Emit(OpCodes.Brfalse, falseBegin);
            Visit(node.IfTrue);
            _context.Generator.Emit(OpCodes.Br, endAll);
            _context.Generator.MarkLabel(falseBegin);
            Visit(node.IfFalse);
            _context.Generator.MarkLabel(endAll);
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Variable) && node.Method.Name == "Wrap")
            {
                //Visit(node.Arguments[0]);
                return base.Visit(node.Object);
            }
            if (node.Method.IsStatic)
            {
                foreach (var param in node.Arguments)
                {
                    Visit(param);
                }

                _context.Generator.Emit(OpCodes.Call, node.Method);
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Type == typeof (Variable))
            {
                var variable = Expression.Lambda(node).Compile().DynamicInvoke() as Variable;
                _context.Generator.Emit(OpCodes.Ldloc, variable.Index);
                return base.VisitMember(node);
            }

            return base.VisitMember(node);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            Visit(node.Operand);

            if (node.Method != null)
            {
                _context.Generator.Emit(OpCodes.Call, node.Method);
            }
            else
            {
                switch (node.NodeType)
                {
                    case ExpressionType.Negate:
                        _context.Generator.Emit(OpCodes.Neg);
                        break;
                    case ExpressionType.Not:
                        _context.Generator.Emit(OpCodes.Not);
                        break;
                    case ExpressionType.Convert:
                        _context.Generator.Emit(OpCodes.Box, node.Operand.Type);                       
                        break;
                    default:
                        throw new NotSupportedException("Not supported");
                }
            }
            return node;
        }



        protected override Expression VisitBinary(BinaryExpression node)
        {
            if (node.Method != null)
            {
                var lhs = Visit(node.Left);
                var rhs = Visit(node.Right);
                _context.Generator.Emit(OpCodes.Call, node.Method);               
            }
            else
            {              
                switch (node.NodeType)
                {
                    case ExpressionType.Equal:
                        Visit(node.Left);
                        Visit(node.Right);
                        _context.Generator.Emit(OpCodes.Ceq);
                        break;
                    case ExpressionType.GreaterThan:
                        Visit(node.Left);
                        Visit(node.Right);
                        _context.Generator.Emit(OpCodes.Cgt);
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        Visit(node.Left);
                        Visit(node.Right);
                        _context.Generator.Emit(OpCodes.Ceq);
                        Visit(node.Left);
                        Visit(node.Right);
                        _context.Generator.Emit(OpCodes.Cgt);
                        _context.Generator.Emit(OpCodes.Or);
                        break;
                    case ExpressionType.LessThan:
                        Visit(node.Left);
                        Visit(node.Right);
                        _context.Generator.Emit(OpCodes.Clt);
                        break;
                    case ExpressionType.LessThanOrEqual:
                        Visit(node.Left);
                        Visit(node.Right);
                        _context.Generator.Emit(OpCodes.Ceq);
                        Visit(node.Left);
                        Visit(node.Right);
                        _context.Generator.Emit(OpCodes.Clt);
                        _context.Generator.Emit(OpCodes.Or);
                        break;

                    case ExpressionType.Add:
                        Visit(node.Left);
                        Visit(node.Right);
                        _context.Generator.Emit(OpCodes.Add);
                        break;
                    case ExpressionType.Subtract:
                        Visit(node.Left);
                        Visit(node.Right);
                        _context.Generator.Emit(OpCodes.Sub);
                        break;
                    case ExpressionType.Divide:
                        Visit(node.Left);
                        Visit(node.Right);
                        _context.Generator.Emit(OpCodes.Div);
                        break;
                    case ExpressionType.Multiply:
                        Visit(node.Left);
                        Visit(node.Right);
                        _context.Generator.Emit(OpCodes.Mul);
                        break;
                    default:
                        throw new Exception("Not supported");
                }
            }

            return node;
        }

        private Expression Unwrap(MethodCallExpression mce)
        {
            return mce.Arguments[0];
        }
    }
}
