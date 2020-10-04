using System;
using System.Collections.Generic;
using System.Text;

namespace VirtualMachine
{
    public class Tokenizer : ITokenizer
    {
        private readonly string _source;
        private int _currentIndex;
        private int _line;
        private char _current => _source[_currentIndex];
        private List<Token> _tokens = new List<Token>();
        private Dictionary<String, TokenType> _commands = new Dictionary<string, TokenType>()
        {
            {"add", TokenType.Add},
            {"sub", TokenType.Sub},
            {"neg", TokenType.Neg},
            {"eq", TokenType.Eq},
            {"gt", TokenType.Gt},
            {"lt", TokenType.Lt},
            {"and", TokenType.And},
            {"or", TokenType.Or},
            {"not", TokenType.Not},
            {"push", TokenType.Push},
            {"pop", TokenType.Pop},
        };
        private Dictionary<String, SegmentType> _segments = new Dictionary<string, SegmentType>()
        {
            {"argument", SegmentType.Argument},
            {"local", SegmentType.Local},
            {"static", SegmentType.Static},
            {"constant", SegmentType.Constant},
            {"this", SegmentType.This},
            {"that", SegmentType.That},
            {"pointer", SegmentType.Pointer},
            {"temp", SegmentType.Temp},
        };

        public Tokenizer(string source)
        {
            _source = source;
        }

        public List<Token> Tokenize()
        {
            while (hasNext())
            {
                switch (_current)
                {
                    case '\n':
                        _line++;
                        break;
                    case ' ':
                    case '\r':
                        break;
                    case '/':
                        if (hasNext() && peek() == '/')
                        {
                            while (hasNext() && peek() != '\n')
                            {
                                advance();
                            }
                        }
                        break;
                    default:
                        if (char.IsDigit(_current))
                        {
                            number();
                            break;
                        }
                        else if (char.IsLetter(_current))
                        {
                            command();
                            break;
                        }
                        
                        throw new Exception("Parsing error");
                };
                advance();
            }

            return _tokens;
        }

        private void command()
        {
            var builder = new StringBuilder();
            builder.Append(_current);

            while (hasNext() && Char.IsLetter(peek()))
            {
                builder.Append(advance());
            }

            var buffer = builder.ToString();
            if (_commands.TryGetValue(buffer, out var tokenType))
            {
                _tokens.Add(new Token(tokenType, _line));
            }
            else
            {
                if (_segments.TryGetValue(buffer, out var segmentType))
                {
                    _tokens.Add(new Token(TokenType.Segment, _line, segmentType));
                }
                else
                {
                    _tokens.Add(new Token(TokenType.Variable, _line, buffer));
                }
            }
            
        }
        private void number()
        {
            var builder = new StringBuilder();
            builder.Append(_current);

            while (hasNext() && Char.IsDigit(peek()))
            {
                builder.Append(advance());
            }

            if (int.TryParse(builder.ToString(), out var result))
            {
                _tokens.Add(new Token(TokenType.Number, _line, result));
                return;
            }
            
            throw new Exception("Parsing error");
        }
        private char advance()
        {
            if (!hasNext())
            {
                throw new IndexOutOfRangeException();
            }
            
            _currentIndex++;
            return _current;
        }
        
        private char release()
        {
            if (!hasPrevious())
            {
                throw new IndexOutOfRangeException();
            }

            _currentIndex--;
            return _current;
        }
        
        private char peek()
        {
            if (!hasNext())
            {
                throw new IndexOutOfRangeException();
            }

            return _source[_currentIndex + 1];
        }
        
        private bool hasNext() => _currentIndex < _source.Length - 1;
        private bool hasPrevious() => _currentIndex > 0 && _source.Length > 0;
    }
}