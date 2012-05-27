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

            
            var title = method.DeclareVariable(typeof (string));
            title.Set(() => "Hello!");
           


            var name = method.DeclareVariable(typeof (string));

            method.Call(typeof (Console), "WriteLine", typeof (string))
                .WithParameter(() => "Enter your name: ");

            name.Set(() => Console.ReadLine());


            var i = method.DeclareVariable(typeof(int));
            i.Set(() => 1);
            method.While(() => i.Wrap<int>() <= 10)
                .Do(x =>
                        {
                            x.Call(typeof (MessageBox), "Show", typeof (string), typeof (string))
                                .WithParameter(() => 
                                               "Hello, " + name.Wrap<string>() + ": "
                                               +
                                               ((1 == i.Wrap<int>())
                                                    ? i.Wrap<int>() + "st"
                                                    : ((2 == i.Wrap<int>())
                                                           ? i.Wrap<int>() + "nd"
                                                           : i.Wrap<int>() + "th")))

                                .WithParameter(() => title.Wrap<string>()); // title

                            i.Set(() => i.Wrap<int>() + 1); // increase counter
                        });



            method.Return(() => "Thi");

            var func = method.Build<Func<string>>() as Func<string>;
            func();
            Console.ReadLine();
        }
    }
}
