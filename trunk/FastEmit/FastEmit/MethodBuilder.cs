using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;


namespace FastEmit
{

    public class Method : Scope
    {
        public Scope ActiveScope;

        private readonly Type[] _parameterTypes;
        private readonly Type _returnType;
        private readonly List<Variable> _variables = new List<Variable>(); 

        /// <summary>
        /// Create a new Method with specified attributes
        /// </summary>
        /// <param name="parameterTypes">Parameter types</param>
        /// <param name="returnType">Return type of the method</param>
        public Method(Type[] parameterTypes, Type returnType)
        {
            ActiveScope = this;
            Method = this;
            _parameterTypes = parameterTypes;
            _returnType = returnType;            
        }

        /// <summary>
        /// Declares a new local variable for the method
        /// </summary>
        /// <param name="type">Type of the variable</param>
        /// <returns>Newly created variable</returns>
        public Variable DeclareVariable(Type type)
        {
            var variable = new Variable()
                               {
                                   Method = this,
                                   Type = type
                               };
            _variables.Add(variable);
            return variable;
        }

        /// <summary>
        /// Compiles the method to delegate
        /// </summary>
        /// <typeparam name="T">Delegate type to compile</typeparam>
        /// <returns>Fresh newly created delegate</returns>
        public Delegate Build<T>()
        {
            var method = new DynamicMethod("", MethodAttributes.Public | MethodAttributes.Static,
                                           CallingConventions.Standard, _returnType, _parameterTypes,
                                           typeof (FastEmit.Method).Module, true);
 
            var generator = method.GetILGenerator();

            _variables.ForEach(x =>
                                   {
                                       x.Index = generator.DeclareLocal(x.Type).LocalIndex;
                                   });

            Emit(new EmitContext() { Generator = generator});
            generator.Emit(OpCodes.Ret);
            return method.CreateDelegate(typeof (T));
        }
    }
}
