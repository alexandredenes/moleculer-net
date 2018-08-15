using ServiceAction.Abstractions;
using System;
using System.Threading.Tasks;

namespace DummyActions
{
    [ServiceAction]
    public class AnotherService
    {
        static int count = 1;

        public int Method3()
        {
            return count++;
        }
    }
}
