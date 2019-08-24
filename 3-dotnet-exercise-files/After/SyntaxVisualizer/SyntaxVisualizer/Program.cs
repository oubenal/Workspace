using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SyntaxVisualizer
{
    class Program
    {
#if DEMO1
        publicstatic void Main(string[] args)
        {
        }
#elif DEMO2
        public/*this is something*/static void Main(string[] args)
        {
        }
#elif DEMO3
        public/*this is something*/static void Main(string[] args)
        {
            // Not a keyword
            await
        }
#elif DEMO4
        public async /*this is something*/static void Main(string[] args)
        {
            // A keyword
            await
        }
#elif DEMO5
        public async /*this is something*/static void Main(string[] args)
        {
            // InvocationExpression
            Console.WriteLine(args);
        }
#elif DEMO6
        public async /*this is something*/static void Main(string[] args)
        {
            // NumericLiteralToken
            123
        }
#else
        static void Main(string[] args)
        {
        }
#endif
    }
}
