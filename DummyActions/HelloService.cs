using ServiceAction.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace DummyActions
{
    [ServiceAction]
    public class HelloService
    {
        private Context _context;

        public HelloService(Context ctx)
        {
            _context = ctx;
        }

        public int Method1(int a, int b)
        {
            return a + b;
        }

        public List<int> Method2()
        {
            List<int> retVal = new List<int>();
            for (int x = 0; x < 20; x++)
                retVal.Add((int)_context.Call("AnotherService.Method3", null, null));

            return retVal;
        }

    }
}
