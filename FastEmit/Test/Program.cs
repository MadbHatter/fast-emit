using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
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
            var testString = method.DeclareVariable(typeof (string));

            testString.Set(() => "abc");

            method.For(() => 1 == 1)
                .Do(x =>
                        {
                            x.Call(typeof (MessageBox), "Show", typeof(string), typeof(string))
                                .WithParameter(() => "test")
                                .WithParameter(() => "abc");
                        });

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
            func();
            Console.ReadLine();
        }
    }
}
