using Console.PrL.Interfaces;
using Core.DataClasses;

namespace Console.PrL.Commands
{
    public abstract class Command
    {
        protected Command(IConsole console)
        {
            this.Console = console;
        }

        public abstract string Name { get; }

        public abstract string Description { get; }

        protected IConsole Console { get; set; }

        public abstract Task<OptionalResult<string>> Execute(string token);
    }
}
