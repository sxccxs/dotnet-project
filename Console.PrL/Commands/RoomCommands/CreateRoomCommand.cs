using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;
using Core.Models.RoomModels;

namespace Console.PrL.Commands.RoomCommands
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
            var userResult = this.authenticationService.GetUserByToken(token);

            if (!userResult.IsSuccess)
            {
                return new OptionalResult<string>(userResult);
            }

            var user = userResult.Value;

            var roomData = this.GetNewRoomData();

            var createResult = this.roomService.CreateRoomForUser(user, roomData);

            if (!createResult.IsSuccess)
            {
                return new OptionalResult<string>(createResult);
            }

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
