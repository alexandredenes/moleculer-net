using Protocol.Abstractions.Messages;
using System;
using System.Collections.Generic;
using System.Text;

namespace Protocol.Abstractions
{
    public interface IServiceExecutor
    {
        object Execute(RequestMessage requestMessage);
    }
}
