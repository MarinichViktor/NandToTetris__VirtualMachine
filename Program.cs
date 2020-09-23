using System;
using System.Collections.Generic;
using System.IO;

namespace VirtualMachine
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("Invalid arguments provided");
            }

            var tokens = new List<Token>();
            var files = HandlePath(args[0]);
            foreach (var file in files)
            {
                using var reader = new StreamReader(File.OpenRead(file));
                var result = reader.ReadToEnd();
                var tokenizer = new Tokenizer(result);
                tokens.AddRange(tokenizer.Tokenize());
            }
            
            var parser = new Parser(tokens);
            var expressions = parser.Parse();
            var res = 123;
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