using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastEmit.Core
{
    public class LambdaExpression : Expression
    {
        private readonly System.Linq.Expressions.Expression _expression;

        public LambdaExpression(Method method, System.Linq.Expressions.Expression expression) : base(method)
        {
            _expression = expression;
        }

        public override void Emit(EmitContext context)
        {
            var visitor = new Visitor(context);
            visitor.Visit(_expression);
        }
    }

    public class ExpressionBuilder<T>
    {
        public ExpressionBuilder(Method method, System.Linq.Expressions.Expression<Func<T>> expression)
        {
            Expression = new LambdaExpression(method, expression);
        }

        public Expression Expression;
    }
}
