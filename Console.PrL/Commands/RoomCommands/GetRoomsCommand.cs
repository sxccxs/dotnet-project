using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace Console.PrL.Commands.RoomCommands
{
    internal class GetRoomsCommand : Command
    {
        private readonly IUserRoomService roomService;

        private readonly IAuthenticationService authenticationService;

        public GetRoomsCommand(IConsole console, IAuthenticationService authenticationService, IUserRoomService roomService)
            : base(console)
        {
            this.roomService = roomService;
            this.authenticationService = authenticationService;
        }

        public override string Name => "/rooms";

        public async override Task<OptionalResult<string>> Execute(string token)
        {
            var userResult = await this.authenticationService.GetUserByToken(token);
            if (!userResult.IsSuccess)
            {
                return new OptionalResult<string>(userResult);
            }

            var user = userResult.Value;
            var rooms = (await this.roomService.GetRoomsForUser(user)).ToList();
            this.Output(rooms);
            return new OptionalResult<string>();
        }

        private void Output(List<RoomModel> rooms)
        {
            this.Console.Print("Rooms you are in:\n");
            this.Console.Print("\n");
            if (rooms.Count == 0)
            {
                this.Console.Print("<It's empty here...>\n");
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                var room = rooms[i];
                this.Console.Print($"{i + 1}) {room.Name}\n");
            }

            this.Console.Print("\n");
        }
    }
}
