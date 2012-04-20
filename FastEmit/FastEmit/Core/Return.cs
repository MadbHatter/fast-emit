using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace FastEmit.Core
{
    public class ReturnStatement : Statement
    {
        private readonly Expression _value;

        public ReturnStatement(Method method, Expression value) : base(method)
        {
            _value = value;
        }

        public override void Emit(EmitContext context)
        {
            if (_value != null) // void?
                _value.Emit(context); 
            context.Generator.Emit(OpCodes.Ret);
        }
    }

    public static class ReturnExtensions
    {
        public static void Return(this Scope scope)
        {
            Return<object>(scope, null);
        }

        public static void Return<T>(this Scope scope, System.Linq.Expressions.Expression<Func<T>> value)
        {
            var builder = new ExpressionBuilder<T>(scope.Method, value);
            scope.Children.Add(new ReturnStatement(scope.Method, builder.Expression));
        }
    }
}
