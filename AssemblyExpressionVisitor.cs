using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace VirtualMachine
{
    public class AssemblyExpressionVisitor : IExpressionVisitor
    {
        private Dictionary<SegmentType, String> _segments = new Dictionary<SegmentType, string>()
        {
            {SegmentType.Argument, "ARG"},
            {SegmentType.Local, "LCL"},
            {SegmentType.This, "This"},
            {SegmentType.That, "That"},
            {SegmentType.That, "That"},
            // TODO: handle next locations
            // {SegmentType.Static, "LCL"},
            // Pointer,
            // Temp,
        };
        
        public object VisitPushExpression(Expression.PushExpression expression)
        {
            var address = expression.Address;
            var segmentType = expression.Segment;
            var buffer = new StringBuilder();

            if (_segments.TryGetValue(segmentType, out var segment))
            {
                buffer.Append($@"@{segment}\n
                    D=A\n
                    @{address}\n
                    A=D+A\n
                    D=M\n
                    @SP\n
                    M=D\n
                    @SP\n
                    M = M + 1\n");

                return buffer.ToString();
            }
            
            throw new EvaluateException("Failed to evaluate push expression");
        }

        public object VisitPopExpression(Expression.PopExpression expression)
        {
            var address = expression.Address;
            var segmentType = expression.Segment;
            var buffer = new StringBuilder();

            if (_segments.TryGetValue(segmentType, out var segment))
            {
                buffer.Append($@"
                    @SP\n
                    M=M-1\n
                    @{segment}\n
                    D=A\n
                    @{address}\n
                    D=D+A\n
                    @R5\n
                    M=D\n
                    @SP\n
                    D=M\n
                    @R5\n
                    M=D\n");

                return buffer.ToString();
            }
            
            throw new EvaluateException("Failed to evaluate pop expression");
        }

        public object VisitCommandExpression(Expression.CommandExpression expression)
        {
            var command = expression.type;
            var buffer = new StringBuilder();

            if (command == TokenType.Add)
            {
                buffer.Append($@"
                    @SP\n
                    D=A


                    @R5
                    M=D
                    @SP\n
                    M=M-1\n
                    D=M


");

                return buffer.ToString();
            }
            
            throw new NotImplementedException();
        }
    }
}