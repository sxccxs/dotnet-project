using Console.PrL.Interfaces;
using Core.DataClasses;

namespace Console.PrL.Commands
{
    internal class HelpCommand : Command
    {
        private Command[] commands;

        public HelpCommand(IConsole console, Command[] commands)
            : base(console)
        {
            this.commands = commands;
            this.commands = this.commands.Append(this).ToArray();
        }

        public override string Name => "/help";

        public override string Description => "Get info about commands.";

        public override Task<OptionalResult<string>> Execute(string token)
        {
            this.Output();
            return Task.FromResult(new OptionalResult<string>());
        }

        private void Output()
        {
            this.Console.Print("Possible commands:\n");
            foreach (var command in this.commands)
            {
                this.Console.Print($"    {command.Name}: {command.Description}\n");
            }
        }
    }
}
