using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace FastEmit.Core
{
    public class CallStatement : Statement
    {
        private Scope _callScope;
        private Type _type;
        private Type[] _paramTypes;
        private string _name;
        private List<Expression> _params;

        public CallStatement(Method method, Type type, string name, params Type[] @params)
            : base(method)
        {
            _type = type;
            _name = name;
            _params = new List<Expression>();
            _paramTypes = @params;
        }

        public CallStatement WithParameter<T>(System.Linq.Expressions.Expression<Func<T>> param)
        {
            var expr = new ExpressionBuilder<T>(Method, param);
            _params.Add(expr.Expression);

            return this;
        }

        public override void Emit(EmitContext context)
        {
            if (_paramTypes.Length != _params.Count)
                throw new Exception("Parameter count mismatch");

            foreach (var param in _params)
                param.Emit(context);

            MethodInfo mInf;
            context.Generator.Emit(OpCodes.Call, (mInf = _type.GetMethod(_name, _paramTypes)));

            if (mInf.ReturnType != typeof(void))
                context.Generator.Emit(OpCodes.Pop);
        }
    }

    public static class CallExtensions
    {
        public static CallStatement Call(this Scope scope, Type type, string name, params Type[] @params)
        {
            var statement = new CallStatement(scope.Method, type, name, @params);
            scope.Children.Add(statement);

            return statement;
        }
    }
}
