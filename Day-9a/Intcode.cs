using System;
using System.Diagnostics;

namespace Day_9a
{
    class Intcode
    {
        private long _pointer;
        private long _relativeBase;
        private long[] _code;

        private const string AddCommand = "01";
        private const string MultiplyCommand = "02";
        private const string InputCommand = "03";
        private const string OutputCommand = "04";
        private const string JumpIfTrueCommand = "05";
        private const string JumpIfFalseCommand = "06";
        private const string LessThanCommand = "07";
        private const string EqualsCommand = "08";
        private const string SetBaseCommand = "09";
        private const string ExitCommand = "99";

        public Intcode(long[] code) => _code = code;

        public void Process()
        {
            while (true)
            {
                var command = ConsumeOpCode().ToString();
                var opCode = command.Length < 2 ? "0" + command : command[^2..];
                var parameters = command.Length > 2 ? command[..^2].PadLeft(4, '0') : "0000";

                if (opCode == AddCommand) Add(parameters);
                else if (opCode == MultiplyCommand) Multiply(parameters);
                else if (opCode == InputCommand) Input(parameters);
                else if (opCode == OutputCommand) Output(parameters);
                else if (opCode == JumpIfTrueCommand) JumpIfTrue(parameters);
                else if (opCode == JumpIfFalseCommand) JumpIfFalse(parameters);
                else if (opCode == LessThanCommand) LessThan(parameters);
                else if (opCode == EqualsCommand) Equals(parameters);
                else if (opCode == SetBaseCommand) SetBase(parameters);
                else if (opCode == ExitCommand) return;
            }
        }

        private void Add(string parameters)
        {
            long a = ConsumeOpCode();
            long b = ConsumeOpCode();
            long target = ConsumeOpCode();

            a = GetParameter(parameters[^1], a);
            b = GetParameter(parameters[^2], b);
            target = GetTarget(parameters[^3], target);

            ExpandingSet(target, a + b);
        }

        private void Multiply(string parameters)
        {
            long a = ConsumeOpCode();
            long b = ConsumeOpCode();
            long target = ConsumeOpCode();

            a = GetParameter(parameters[^1], a);
            b = GetParameter(parameters[^2], b);
            target = GetTarget(parameters[^3], target);

            ExpandingSet(target, a * b);
        }

        private void Input(string parameters)
        {
            long target = ConsumeOpCode();
            target = GetTarget(parameters[^1], target);

            Console.WriteLine($"Waiting for input at ({target}): ");
            long input = Convert.ToInt32(Console.ReadLine());

            ExpandingSet(target, input);
        }

        private void Output(string parameters)
        {
            long target = ConsumeOpCode();

            long value = GetParameter(parameters[^1], target);

            Console.WriteLine($"Value: {value}");
        }

        private void JumpIfTrue(string parameters)
        {
            long condition = ConsumeOpCode();
            long target = ConsumeOpCode();

            condition = GetParameter(parameters[^1], condition);
            target = GetParameter(parameters[^2], target);

            if (condition != 0) _pointer = target;
        }

        private void JumpIfFalse(string parameters)
        {
            long condition = ConsumeOpCode();
            long target = ConsumeOpCode();

            condition = GetParameter(parameters[^1], condition);
            target = GetParameter(parameters[^2], target);

            if (condition == 0) _pointer = target;
        }

        private void LessThan(string parameters)
        {
            long a = ConsumeOpCode();
            long b = ConsumeOpCode();
            long target = ConsumeOpCode();

            a = GetParameter(parameters[^1], a);
            b = GetParameter(parameters[^2], b);
            target = GetTarget(parameters[^3], target);

            ExpandingSet(target, a < b ? 1 : 0);
        }

        private void Equals(string parameters)
        {
            long a = ConsumeOpCode();
            long b = ConsumeOpCode();
            long target = ConsumeOpCode();

            a = GetParameter(parameters[^1], a);
            b = GetParameter(parameters[^2], b);
            target = GetTarget(parameters[^3], target);

            ExpandingSet(target, a == b ? 1 : 0);
        }

        private void SetBase(string parameters)
        {
            long amount = ConsumeOpCode();

            amount = GetParameter(parameters[^1], amount);

            _relativeBase += amount;
        }

        private long GetParameter(char mode, long value)
        {
            return mode switch
            {
                '0' => ExpandingGet(value),
                '1' => value,
                '2' => ExpandingGet(_relativeBase + value)
            };
        }

        private long GetTarget(char mode, long value)
        {
            return mode switch
            {
                '0' => value,
                '1' => throw new Exception("Target params cannot be set to immediate mode."),
                '2' => _relativeBase + value
            };
        }

        private void ExpandingSet(long index, long value)
        {
            if (index >= _code.Length)
                Array.Resize(ref _code, (int)index + 1);
            _code[index] = value;
        }

        public long ExpandingGet(long index)
        {
            if (index >= _code.Length)
                Array.Resize(ref _code, (int)index + 1);
            return _code[index];
        }

        private long ConsumeOpCode() => _pointer < _code.Length ? _code[_pointer++] : 99;
    }
}
