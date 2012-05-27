using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using FastEmit.Core;

namespace FastEmit
{
    public class Variable
    {
        public Method Method;
        public int Index;
        public Type Type;

        public void Set<T>(System.Linq.Expressions.Expression<Func<T>> value)
        {
            var builder = new Core.ExpressionBuilder<T>(Method, value);
            Method.ActiveScope.Children.Add(new VariableSetEmit(this, builder.Expression));
        }

        public T Wrap<T>()
        {
            return default(T);
        }
    }

    public class VariableSetEmit : IEmittable
    {
        private readonly Variable _variable;
        private readonly Core.Expression _value;

        public VariableSetEmit(Variable variable, Core.Expression value)
        {
            _variable = variable;
            _value = value;
        }

        public void Emit(EmitContext context)
        {
            _value.Emit(context);
            context.Generator.Emit(OpCodes.Stloc, _variable.Index);
        }
    }

    public class VariableGetEmit : Core.Expression
    {
        private readonly Variable _variable;

        public VariableGetEmit(Method method, Variable variable) : base(method)
        {
            _variable = variable;
        }

        public override void Emit(EmitContext context)
        {
            context.Generator.Emit(OpCodes.Ldloc, _variable.Index);
        }
    }
}
