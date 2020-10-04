using System;
using static VirtualMachine.AsmBuilder;

namespace VirtualMachine
{
    public class Interpreter : IExpressionVisitor
    {
        public static int Counter = 0;
        private string _context;

        public Interpreter(string context) => _context = context;

        public object Visit(params Expression[] expressions)
        {

            throw new NotImplementedException();
        }

        public object VisitPushExpression(Expression.PushExpression expression)
        {
            var address = expression.Address;
            var segmentType = expression.Segment;

            return new AsmBuilder(_context)
                    .LoadSegmentValueIntoD(segmentType, address)
                    .PushDOnStack()
                    .Build();
        }

        public object VisitPopExpression(Expression.PopExpression expression)
        {
            var address = expression.Address;
            var segmentType = expression.Segment;

            return new AsmBuilder(_context)
                .PopDFromStack()
                .LoadDIntoSegment(segmentType, address)
                .Build();
        }

        public object VisitCommandExpression(Expression.CommandExpression expression)
        {
            var builder = new AsmBuilder(_context)
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
                            builder.AssignD(Command.DOrM);
                            break;
                        case TokenType.Eq:
                            var checkEqLabel = $"CheckEquality.${Counter}";
                            var eqLabel = $"Equal.${Counter}";

                            builder
                                .AssignD(Command.DMinusM)
                                .LoadA(checkEqLabel)
                                .JMP()

                                .Label(eqLabel)
                                .AssignD(Command.MinusOne)
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
                                .AssignD(Command.MinusOne)
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
                                .AssignD(Command.MinusOne)
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
                        .LoadA("0")
                        .AssignD(Command.AMinusD);
                    break;
            }
                
            return builder
                .PushDOnStack()
                .Build();
        }
    }
}

