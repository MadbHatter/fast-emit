using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;

namespace FastEmit
{
    public class Scope : IEmittable
    {
        public Method Method;
        public List<IEmittable> Children = new List<IEmittable>(); 

        public void Emit(EmitContext context)
        {
            Children.ForEach(x => x.Emit(context));
        }
    }
}
