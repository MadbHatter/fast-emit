using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastEmit.Core
{
    /// <summary>
    /// Piece of code which does not return a value and can be child of a Scope
    /// </summary>
    public abstract class Statement : IEmittable
    {
        public Method Method;
        protected Statement(Method method)
        {
            Method = method;
        }

        public abstract void Emit(EmitContext context);
    }
}
