using System;

namespace Tests
{
    class Baz
    {
        public void Qux(int x, int y, int z)
        {
            var res = (x * y) + ((int)z - 1);
            System.Console.WriteLine(res);
        }
    }
}
