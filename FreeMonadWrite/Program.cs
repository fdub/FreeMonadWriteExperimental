using System;
using System.Linq;
using Microsoft.FSharp.Core;
using FreeActions;
using Actions = FreeActions.Actions;

namespace FreeMonadWrite
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            var value = "initial";
            var program = Actions.abort;
            var writer = new Writer(s => value = s);
            var running = true;

            do
            {
                (running, program) = Process(Console.ReadLine(), writer, program);
            } while (running);

            Actions.interpret(program);
            Console.WriteLine(value);
        }

        public static (bool running, ActionProgram<Unit> newProgram) Process(
            string command,
            Writer writer,
            ActionProgram<Unit> program)
        {
            if (command.Any())
            {
                switch (command[0])
                {
                    case 'w':
                    case 'W':
                        return (true, Actions.bindInvoke(() => writer.Write(command.Substring(1)), program));
                    case 'u':
                    case 'U':
                        return (true, Actions.unbind(program));
                    case 'a':
                    case 'A':
                        return (false, Actions.abort);
                }
            }

            return (false, program);
        }
    }

    public class Writer
    {
        private readonly Action<string> _commitAction;

        public Writer(Action<string> commitAction)
        {
            _commitAction = commitAction;
        }

        public void Write(string s)
        {
            Console.WriteLine($"--> Write({s})");
            _commitAction(s);
        }
    }
}