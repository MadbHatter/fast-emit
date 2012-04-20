using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FastEmit
{
    public interface IEmittable
    {
        void Emit(EmitContext context);
    }
}
