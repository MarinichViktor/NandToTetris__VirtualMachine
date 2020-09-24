using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static VirtualMachine.AsmBuilder;

namespace VirtualMachine
{
    public class AssemblyExpressionVisitor : IExpressionVisitor
    {
        AssemblyBuilder _builder = new AssemblyBuilder();
        public static int Counter = 0;

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

        public object Visit(params Expression[] expressions)
        {

            throw new NotImplementedException();
        }

        public object VisitPushExpression(Expression.PushExpression expression)
        {
            // var builder = new AssemblyBuilder();
            // builder.PushOnSegment(expression.Segment, expression.Address);

            // return builder.Build();
            var address = expression.Address;
            var segmentType = expression.Segment;
            // var buffer = new StringBuilder();
            // var assembler = new AsmBuilder();

            // if (_segments.TryGetValue(segmentType, out var segment))
            // {

            return new AsmBuilder()
                    .LoadSegmentValueIntoD(segmentType, address)
                    .PushDOnStack()
                    .Build();
                
            // buffer.AppendLine($@"@{segment}
            //     D=M
            //     @{address}
            //     A=D+A
            //     D=M
            //     @SP
            //     A=M
            //     M=D
            //     @SP
            //     M=M+1");
            //
            //     return buffer.ToString();
            // } else if (segmentType == SegmentType.Constant) {
            //     buffer.AppendLine($@"@{address}
            //         D=A
            //         @SP
            //         A=M
            //         M=D
            //         @SP
            //         M=M+1");
            //
            //     return buffer.ToString();
            // }
        }

        public object VisitPopExpression(Expression.PopExpression expression)
        {
            // var builder = new AssemblyBuilder();
            // builder.PopOnSegment(expression.Segment, expression.Address);
            //
            // return builder.Build();
            var address = expression.Address;
            var segmentType = expression.Segment;

            return new AsmBuilder()
                .PopDFromStack()
                .LoadDIntoSegment(segmentType, address)
                .Build();

            // if (_segments.TryGetValue(segmentType, out var segment))
            // {
            //     buffer.AppendLine($@"@SP
            //         M=M-1
            //         @{segment}
            //         D=M
            //         @{address}
            //         D=D+A
            //         @R5
            //         M=D
            //         @SP
            //         A=M
            //         D=M
            //         @R5
            //         A=M
            //         M=D");
            //
            //     return buffer.ToString();
            // }
            //
            // throw new EvaluateException("Failed to evaluate pop expression");
        }

        public object VisitCommandExpression(Expression.CommandExpression expression)
        {
            var builder = new AsmBuilder()
                .PopDFromStack()
                .LoadA(Register.R5)
                .AssignM(Command.D);

            switch (expression.type)
            {
                // Binary
                case TokenType.Add:
                case TokenType.Sub:
                case TokenType.And:
                case TokenType.Or:
                case TokenType.Eq:
                case TokenType.Lt:
                case TokenType.Gt:
                    builder
                        .PopDFromStack()
                        .LoadA(Register.R5);
                    var endLabel = $"End.${Counter}";
                    Counter++;

                    switch (expression.type)
                    {
                        case TokenType.Add:
                            builder.AssignD(Command.DPlusM);
                            break;
                        case TokenType.Sub:
                            builder.AssignD(Command.DMinusM);
                            break;
                        case TokenType.And:
                            builder.AssignD(Command.DAndM);
                            break;
                        case TokenType.Or:
                            builder.AssignD(Command.DOrA);
                            break;
                        case TokenType.Eq:
                            var checkEqLabel = $"CheckEquality.${Counter}";
                            var eqLabel = $"Equal.${Counter}";
                            
                            builder
                                .AssignD(Command.DMinusM)
                                .LoadA(checkEqLabel)
                                .JMP()

                                .Label(eqLabel)
                                .AssignD(Command.One)
                                .LoadA(endLabel)
                                .JMP()

                                .Label(checkEqLabel)
                                .LoadA(eqLabel)
                                .JEQD()

                                .AssignD(Command.Zero)

                                .Label(endLabel);
                            break;
                        case TokenType.Lt:
                            var checkLtLabel = $"CheckLt.${Counter}";
                            var ltLabel = $"Lt.${Counter}";
                            builder
                                .AssignD(Command.DMinusM)
                                .LoadA(checkLtLabel)
                                .JMP()

                                .Label(ltLabel)
                                .AssignD(Command.One)
                                .LoadA(endLabel)
                                .JMP()

                                .Label(checkLtLabel)
                                .LoadA(ltLabel)
                                .JLTD()

                                .AssignD(Command.Zero)

                                .Label(endLabel);
                            break;
                        case TokenType.Gt:
                            var checkGtLabel = $"CheckTt.${Counter}";
                            var gtLabel = $"Gt.${Counter}";
                            builder
                                .AssignD(Command.DMinusM)
                                .LoadA(checkGtLabel)
                                .JMP()

                                .Label(gtLabel)
                                .AssignD(Command.One)
                                .LoadA(endLabel)
                                .JMP()

                                .Label(checkGtLabel)
                                .LoadA(gtLabel)
                                .JGTD()

                                .AssignD(Command.Zero)

                                .Label(endLabel);
                            break;
                    }
                break;
                // UNARY
                case TokenType.Not:
                    builder
                        .AssignD(Command.NotD);
                    break;
                case TokenType.Neg:
                    builder
                        .AssignA("0")
                        .AssignD(Command.AMinusD);
                    break;
            }
                
            return builder
                .PushDOnStack()
                .Build();
            
            // if (command == TokenType.Add)
            // {
            //     buffer.AppendLine($@"@SP
            //         M=M-1
            //         A=M
            //         D=M
            //         @R5
            //         M=D
            //         @SP
            //         M=M-1
            //         A=M
            //         D=M
            //         @R6
            //         M=D
            //         @R6
            //         D=M
            //         @R5
            //         D=D+M
            //         @SP
            //         A=M
            //         M=D
            //         @SP
            //         M=M+1"
            //     );
            //
            //     return buffer.ToString();
            // }
            //
            // throw new NotImplementedException();
        }
    }
}

