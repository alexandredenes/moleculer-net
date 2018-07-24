using ServiceAction.Abstractions;
using System;
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

        public async Task<int> Method2()
        {
            var result = await _context.Call("service2.hello", null, null);

            return 4;
        }

    }
}
