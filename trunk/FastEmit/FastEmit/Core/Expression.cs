using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastEmit.Core
{
    /// <summary>
    /// Piece of code which returns some value and cannot be used in plain scope
    /// </summary>
    public abstract class Expression : IEmittable
    {
        public Method Method;
        protected Expression(Method method)
        {
            Method = method;
        }

        public abstract void Emit(EmitContext context);
    }
}
