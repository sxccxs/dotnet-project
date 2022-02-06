using System.Data.Common;
using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace Console.PrL.Commands.RoomCommands
{
    internal class AddUserToRoomCommand : Command
    {
        private readonly IUserRoomService userRoomService;
        private readonly IAuthenticationService authenticationService;
        private readonly IRoomService roomService;
        private readonly IUserService userService;

        public AddUserToRoomCommand(
            IConsole console,
            IAuthenticationService authenticationService,
            IRoomService roomService,
            IUserService userService,
            IUserRoomService userRoomService)
            : base(console)
        {
            this.roomService = roomService;
            this.userService = userService;
            this.userRoomService = userRoomService;
            this.authenticationService = authenticationService;
        }

        public override string Name => "/addUserToRoom";

        public override string Description => "Add a new user to room. You must be logged in.";

        public async override Task<OptionalResult<string>> Execute(string token)
        {
            var userResult = await this.authenticationService.GetUserByToken(token);

            if (!userResult.IsSuccess)
            {
                return new OptionalResult<string>(userResult);
            }

            var user = userResult.Value;

            var rooms = (await this.userRoomService.GetRoomsForUser(user)).ToList();
            var apply = this.OutputAvailableRooms(rooms);

            if (apply)
            {
                var roomIdResult = this.GetRoom(rooms);
                if (!roomIdResult.IsSuccess)
                {
                    return new OptionalResult<string>(roomIdResult);
                }

                var room = (await this.roomService.GetByCondition(x => x.Id.Value == roomIdResult.Value)).FirstOrDefault();
                var users = (await this.userRoomService.GetUsersInRoom(room)).ToList();

                var addUserId = this.GetUserToBeAddedId(users);

                var result = await this.userRoomService.AddUserToRoom(roomIdResult.Value, addUserId.Id);
                if (result.IsSuccess)
                {
                    this.Console.Print($"User {addUserId} added successfully\n");
                    return new OptionalResult<string>();
                }

                return new OptionalResult<string>(result);
            }

            return new OptionalResult<string>(false);
        }

        private bool OutputAvailableRooms(List<RoomModel> rooms)
        {
            var apply = true;
            this.Console.Print("Rooms you are in:\n");
            this.Console.Print("\n");
            if (rooms.Count == 0)
            {
                this.Console.Print("<You don't have any rooms to edit>\n");
                apply = false;
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                var room = rooms[i];
                this.Console.Print($"{i + 1}) {room.Name}\n");
            }

            this.Console.Print("\n");
            return apply;
        }

        private OptionalResult<int> GetRoom(List<RoomModel> rooms)
        {
            while (true)
            {
                var parsed = int.TryParse(
                    this.Console.Input("Which room would you like to add a user to? "), out var number);
                if (rooms[number - 1].Id.IsSuccess)
                {
                    return rooms[number - 1].Id;
                }

                this.Console.Print("Enter a valid number.");
            }
        }

        private async Task<OptionalResult<int>> GetUserToBeAddedId(List<UserModel> users)
        {
            while (true)
            {
                var email = this.Console.Input("What is the e-mail of the user you would like to add?: ");
                if (!string.IsNullOrWhiteSpace(email))
                {
                    var user = (await this.userService.GetByCondition(x => x.Email == email)).FirstOrDefault();
                    if (user.Id.IsSuccess)
                    {
                        return user.Id;
                    }
                }

                this.Console.Print("Enter a valid e-mail.");
            }
        }
    }
}
