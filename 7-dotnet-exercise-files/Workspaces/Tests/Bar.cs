using System;

namespace Workspaces
{
    public class Bar
    {
        public static void Foo()
        {
            var get = Qux * 2;
            Console.WriteLine("The answer is {0}", /* answer */ get);
        }

        public static int Qux
        {
            get
            {
                return 42;
            }
        }

        public static void Baz()
        {
            Foo();
        }
    }
}
