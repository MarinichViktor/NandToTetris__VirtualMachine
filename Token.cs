using System;

namespace VirtualMachine
{
    public class Token
    {
        public TokenType Type { get; }
        public int Line { get; }
        public Object Payload { get; }
        
        public Token(TokenType type, int line, object payload = null)
        {
            Type = type;
            Line = line;
            Payload = payload;
        }
    }
}
