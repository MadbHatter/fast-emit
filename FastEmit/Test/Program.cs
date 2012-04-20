using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FastEmit;
using FastEmit.Core;
namespace Test
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var method = new Method(new Type[] {}, typeof (string));
            
            var ifStatus = method.DeclareVariable(typeof (bool));

            method.If(() => 1 == 1)
                .Then(
                    x =>
                        {
                            ifStatus.Set(() => true);
                        })
                .Else(
                    x =>
                        {
                            ifStatus.Set(() => false);

                        });
     
            method.Return(() => "Thi" );

            var func = method.Build<Func<string>>() as Func<string>;
            Console.WriteLine(func());
            Console.ReadLine();
        }
    }
}
