using System;
using System.Collections.Generic;
using System.Linq;

namespace VirtualMachine
{
    public class Parser : IParser
    {
        public List<Token> _tokens { get; }
        private List<Expression> _expressions { get; set; } = new List<Expression>();
        private int _currentIndex = -1;
        private Token _current => _tokens[_currentIndex];

        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public List<Expression> Parse()
        {
            while (hasNext())
            {
                if (match(TokenType.Push, TokenType.Pop))
                {
                    var command = _current;
                    var segment = match(TokenType.Segment) ? _current : null;
                    var address = match(TokenType.Variable, TokenType.Number) ? _current : null;

                    if (segment != null && address != null)
                    {
                        if (command.Type == TokenType.Push)
                        {
                            // TODO: resolve literal payload address in case if variable
                            _expressions.Add(new Expression.PushExpression((SegmentType) segment.Payload,
                                (String) address.Payload));
                        }
                        else
                        {
                            // TODO: resolve literal payload address in case if variable
                            _expressions.Add(new Expression.PopExpression((SegmentType) segment.Payload,
                                (String) address.Payload));
                        }

                        continue;
                    }

                    throw new Exception("Parsing exception");
                }
                else if (match(TokenType.Add, TokenType.And, TokenType.Eq, TokenType.Gt, TokenType.Lt, TokenType.Neg,
                    TokenType.Not, TokenType.Or, TokenType.Sub))
                {
                    _expressions.Add(new Expression.CommandExpression(_current.Type));
                }
                else
                {
                    throw new Exception("Parsing exception");
                }
            }

            return _expressions;
        }

        private bool match(params TokenType[] types)
        {
            if (types.Contains(peek().Type))
            {
                advance();
                return true;
            }

            return false;
        }

        private Token advance()
        {
            if (!hasNext())
            {
                throw new IndexOutOfRangeException();
            }

            return _tokens[++_currentIndex];
        }

        private Token release()
        {
            if (!hasPrevious())
            {
                throw new IndexOutOfRangeException();
            }

            return _tokens[--_currentIndex];
        }

        private Token peek()
        {
            if (!hasNext())
            {
                throw new IndexOutOfRangeException();
            }

            return _tokens[_currentIndex + 1];
        }

        private bool hasNext() => _currentIndex < _tokens.Count - 1;
        private bool hasPrevious() => _currentIndex > 0 && _tokens.Count > 0;
    }
}