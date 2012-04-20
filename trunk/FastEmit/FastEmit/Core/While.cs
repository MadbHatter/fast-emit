using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace FastEmit.Core
{
    public class WhileLoop : Statement
    {
        private readonly Expression _condition;
        private Scope _loopBlock;

        public WhileLoop(Method method, Expression condition) : base(method)
        {
            _condition = condition;
        }

        public WhileLoop Do(Action<Scope> scope)
        {
            _loopBlock = new Scope();
            var oldScope = Method.ActiveScope;

            Method.ActiveScope = _loopBlock;

            scope(_loopBlock);

            Method.ActiveScope = oldScope;

            return this;
        }

        public override void Emit(EmitContext context)
        {
            if(_loopBlock == null)
                throw new InvalidOperationException("Loop block must be set");

            var loopStart = context.Generator.DefineLabel();
            var conditionStart = context.Generator.DefineLabel();

            context.Generator.Emit(OpCodes.Br, conditionStart);
            context.Generator.MarkLabel(loopStart);

            _loopBlock.Emit(context); // Inside loop

            context.Generator.MarkLabel(conditionStart); // to skip first iteration on false condition
            _condition.Emit(context); // Loop condition

            context.Generator.Emit(OpCodes.Brtrue, loopStart);
        }

    }

    public static class ForExtensions
    {
        public static WhileLoop While(this Scope scope, System.Linq.Expressions.Expression<Func<bool>> condition)
        {
            var builder = new ExpressionBuilder<bool>(scope.Method, condition);
            var statement = new WhileLoop(scope.Method, builder.Expression);
            scope.Children.Add(statement);

            return statement;
        }
    }
}
