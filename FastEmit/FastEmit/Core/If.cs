using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace FastEmit.Core
{
    public class IfStatement : Statement
    {
        private readonly Expression _condition;
        private Scope _trueScope;
        private Scope _falseScope;

        public IfStatement(Method method, Expression condition) : base(method)
        {
            _condition = condition;
        }

        public IfStatement Then(Action<Scope> scope)
        {
            _trueScope = new Scope();
            var oldScope = Method.ActiveScope;
            Method.ActiveScope = _trueScope;
            scope(_trueScope);
            Method.ActiveScope = oldScope;

            return this;
        }

        public IfStatement Else(Action<Scope> scope)
        {
            _falseScope = new Scope();
            var oldScope = Method.ActiveScope;
            Method.ActiveScope = _falseScope;
            scope(_falseScope);
            Method.ActiveScope = oldScope;

            return this;
        }

        public override void Emit(EmitContext context)
        {
            if (_trueScope == null)
                throw new InvalidOperationException("True block must be set");

            var endAllLabel = context.Generator.DefineLabel();
            var endTrueLabel = context.Generator.DefineLabel();
            _condition.Emit(context);
            context.Generator.Emit(OpCodes.Brfalse, endTrueLabel);
            _trueScope.Emit(context); // then
            if (_falseScope != null)
                context.Generator.Emit(OpCodes.Br, endAllLabel);
            context.Generator.MarkLabel(endTrueLabel);
            if (_falseScope != null)
                _falseScope.Emit(context);
            if (_falseScope != null)
                context.Generator.MarkLabel(endAllLabel);
        }
    }

    public static class IfExtensions
    {
        public static IfStatement If(this Scope scope, System.Linq.Expressions.Expression<Func<bool>> condition)
        {
            var builder = new ExpressionBuilder<bool>(scope.Method, condition);
            var statement = new IfStatement(scope.Method, builder.Expression);
            scope.Children.Add(statement);
            return statement;
        }
    }
}
