using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;
using Core.Models.RoomModels;

namespace Console.PrL.Commands
{
    internal class CreateRoomCommand : Command
    {
        private readonly IUserRoomService roomService;

        private readonly IAuthenticationService authenticationService;

        public CreateRoomCommand(IConsole console, IAuthenticationService authenticationService, IUserRoomService roomService)
            : base(console)
        {
            this.roomService = roomService;
            this.authenticationService = authenticationService;
        }

        public override string Name => "/createRoom";

        public override OptionalResult<string> Execute(string token)
        {
            var user = this.authenticationService.GetUserByToken(token);

            var roomData = this.GetNewRoomData();

            this.roomService.CreateRoomForUser(user, roomData);

            this.Console.Print($"Room {roomData.Name} was successfully created\n");

            return new OptionalResult<string>();
        }

        private RoomCreateModel GetNewRoomData()
        {
            this.Console.Print("\n");
            var roomData = new RoomCreateModel()
            {
                Name = this.Console.Input("Room name: "),
            };

            this.Console.Print("\n");

            return roomData;
        }
    }
}
