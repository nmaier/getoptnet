using System;
using System.Collections.Generic;
using System.Text;
using NMaier.GetOptNet;
using System.Threading;

namespace TestApp
{
    class Program
    {
        [GetOptOptions(OnUnknownArgument = UnknownArgumentsAction.Throw, UsageEpilog = "That's all, folks")]
        class Opts : GetOpt
        {

            [Parameters(Min = 1, Max = 3)]
            public List<String> Parameters = new List<string>();
            //public string[] Parameters = new string[0];

            [Argument(Helptext = "Provide some string")]
            public string Str = "some string";

            [ArgumentAlias("multi")]
            [Argument("multiargument", Helptext = "You may provide multiple strings", Helpvar = "list item")]
            [ArgumentAlias("multiarg")]
            [ArgumentAlias("ma")]
            [ShortArgument('m')]
            public List<String> Multi = new List<string>();


            [Argument(Helptext = "You may provide multiple Arguments, all of which must be integers")]
            [ShortArgument('a')]
            [ShortArgumentAlias('k')]
            public int[] Arr = new int[0];

            [Argument(Helptext = "LONG NAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAMEEEEEEEE indeed!")]
            [ShortArgument('b')]
            public bool Flag = false;


            [Argument(Helptext = "Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text, Help text")]
            [Counted]
            [ShortArgument('c')]
            public UInt64 Counter = 0;

            private string _prop = "a chicken crossed the road";
            [Argument(Helptext = "Property taking a string that must start with 'a'")]
            [ShortArgument('d')]
            [ArgumentAlias("foo")]
            public string Prop
            {
                get { return _prop; }
                set
                {
                    if (!value.StartsWith("a"))
                    {
                        throw new InvalidValueException("Value for Prop must start with 'a'");
                    }
                    _prop = value;
                }
            }

            [Argument(Helptext = "Integers anyone")]
            public int SomeInt = 1;

            [Argument(Helptext = "Doubles anyone")]
            public double SomeDouble = 0.1;

            [Argument(Required = true)]
            [ShortArgument('r')]
            public string required = null;

            [Argument]
            [FlagArgument(true)]
            public bool defaultFalse = true;

            [Argument]
            [FlagArgument(false)]
            public bool defaultTrue = false;

            [Argument]
            [MultipleArguments(Exact = 2)]
            public string[] requiredTwo = null;
        }

        static void Main(string[] args)
        {
            Console.WriteLine("GetOptNet test program");
            Console.WriteLine("Copyright for this test program is disclaimed. Public domain");
            Console.WriteLine();

            Opts opts = new Opts();
            try
            {
                opts.Parse(args);
                Console.WriteLine("Printing Input");
                Console.WriteLine("Str: " + opts.Str);
                Console.WriteLine("Flag: " + opts.Flag);
                Console.WriteLine("Counter: " + opts.Counter);
                Console.WriteLine("Validated prop: " + opts.Prop);
                Console.WriteLine("SomeInt: " + opts.SomeInt);
                Console.WriteLine("SomeDouble: " + opts.SomeDouble);
                Console.WriteLine("DefaultFalse: " + opts.defaultFalse);
                Console.WriteLine("DefaultTrue: " + opts.defaultTrue);

                foreach (int p in opts.Arr)
                {
                    Console.WriteLine("Arr: {0}", p);
                }


                foreach (string p in opts.Multi)
                {
                    Console.WriteLine("Multi: {0}", p);
                }

                foreach (string p in opts.Parameters)
                {
                    Console.WriteLine("Param: {0}", p);
                }
                Console.WriteLine();
                opts.PrintUsage();
            }
            catch (GetOptException ex)
            {
                Console.WriteLine("Unfortunatly there is a problem, sir");
                Console.WriteLine("--> {0}", ex.Message);
                Console.WriteLine();
                opts.PrintUsage();
            }
        }
    }
}
