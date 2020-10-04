using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace VirtualMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("Invalid arguments provided");
            }

            var buffer = new StringBuilder();
            var files = HandlePath(args[0]);

            foreach (var file in files)
            {
                using var reader = new StreamReader(File.OpenRead(file));
                var result = reader.ReadToEnd();
                var compiled = CompileVmProgram(file.Split("/").Last()?.Split('.').First() ?? "Global", result);
                buffer.AppendLine(compiled);
            }

            buffer.AppendLine("(END)");
            buffer.AppendLine("@END");
            buffer.AppendLine("0;JMP");

            if (File.Exists(args[1]))
            {
                File.Delete(args[1]);
            }
            using (var fs = File.OpenWrite(args[1]))
            {
                var bytes = Encoding.UTF8.GetBytes(buffer.ToString());
                fs.Write(bytes);
            }
        }

        static string CompileVmProgram(string context, string source)
        {
            var tokenizer = new Tokenizer(source);
            var tokens = tokenizer.Tokenize();

            var parser = new Parser(tokens);
            var expressions = parser.Parse();
            var expressionEvaluator = new Interpreter(context);

            var buffer = new StringBuilder();
            foreach (var expression in expressions)
            {
                buffer.AppendLine(expression.Accept(expressionEvaluator) as String);
            }

            return buffer.ToString();
        }

        static List<String> HandlePath(string path)
        {
            if (path.EndsWith(".vm") && File.Exists(path))
            {
                return new List<string>() {path};
            } else if (Directory.Exists(path))
            {
                var files = new List<string>();
                foreach (var file in Directory.GetFiles(path))
                {
                    if (file.EndsWith(".vm"))
                    {
                        files.Add(file);
                    }
                }

                return files;
            }
            
            throw new ArgumentException("File or directory not found");
        }
    }
}
