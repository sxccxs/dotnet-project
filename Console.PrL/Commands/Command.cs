using Console.PrL.Interfaces;
using Core.DataClasses;

namespace Console.PrL.Commands
{
    internal abstract class Command
    {
        public Command(IConsole console)
        {
            this.Console = console;
        }

        public abstract string Name { get; }

        protected IConsole Console { get; }

        public abstract OptionalResult<string> Execute();
    }
}
