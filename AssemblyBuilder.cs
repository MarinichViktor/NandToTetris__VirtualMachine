using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace VirtualMachine
{    public interface IAssemblyBuilder
    {
        AssemblyBuilder PushOnSegment(SegmentType segment, String addressOrIndex);
        String Build();
    }
    
    public class AssemblyBuilder : IAssemblyBuilder
    {
        StringBuilder _commandBuffer = new StringBuilder();

        private Dictionary<SegmentType, String> _segments = new Dictionary<SegmentType, string>()
        {
            {SegmentType.Argument, "ARG"},
            {SegmentType.Local, "LCL"},
            {SegmentType.This, "This"},
            {SegmentType.That, "That"},
            // TODO: handle next locations
            // {SegmentType.Static, "LCL"},
            // Pointer,
            // Temp,
        };
        private Dictionary<TokenType, String> _commands = new Dictionary<TokenType, string>()
        {
            {TokenType.Add, "M+D"},
            {TokenType.Sub, "D-M"},
            // TODO: handle next locations
            // {SegmentType.Static, "LCL"},
            // Pointer,
            // Temp,
        };
        public AssemblyBuilder PopOnSegment(SegmentType segment, String addressOrIndex)
        {
            var segmentLiteral = _segments[segment];
            _commandBuffer.AppendLine($@"@SP
                M=M-1
                @{segmentLiteral}
                D=M
                @{addressOrIndex}
                D=D+A
                @R5
                M=D
                @SP
                A=M
                D=M
                @R5
                A=M
                M=D");

            return this;
        }
        
        public AssemblyBuilder PushOnSegment(SegmentType segment, String addressOrIndex)
        {

            switch (segment)
            {
                
                case SegmentType.Constant:
                    _commandBuffer.AppendLine($@"@{addressOrIndex}
                    D=A
                    @SP
                    A=M
                    M=D
                    @SP
                    M=M+1");

                    break;
                case SegmentType.Local:
                case SegmentType.Argument:
                case SegmentType.This:
                case SegmentType.That:
                    var segmentLiteral = _segments[segment];
                    _commandBuffer.AppendLine($@"@{segmentLiteral}
                        D=M
                        @{addressOrIndex}
                        A=D+A
                        D=M
                        @SP
                        A=M
                        M=D
                        @SP
                        M=M+1");
                    break;
            }
            return this;
        }

        public AssemblyBuilder command(TokenType command)
        {
            switch (command)
            {
                case TokenType.Add:
                    _commandBuffer.AppendLine($@"@SP
                        M=M-1
                        A=M
                        D=M
                        @R5
                        M=D
                        @SP
                        M=M-1
                        A=M
                        D=M
                        @R6
                        M=D
                        @R6
                        D=M
                        @R5
                        D=D+M
                        @SP
                        A=M
                        M=D
                        @SP
                        M=M+1");
                    break;
            }

            return this;
        }

        public string Build() => _commandBuffer.ToString();
    }
}