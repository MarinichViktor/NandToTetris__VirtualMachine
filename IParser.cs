using System.Collections.Generic;

namespace VirtualMachine
{
    public interface IParser
    {
        List<Token> Parse();
    }
}