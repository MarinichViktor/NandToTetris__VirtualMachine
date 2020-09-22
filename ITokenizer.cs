using System.Collections.Generic;

namespace VirtualMachine
{
    public interface ITokenizer
    {
        public List<Token> Tokenize();
    }
}