using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace Console.PrL.Commands
{
    internal class DeleteUserFromRoomCommand : Command
    {
        private readonly IUserService userService;
        private readonly IUserRoomService roomService;
        private readonly IRoomService room1Service;
        private readonly IAuthenticationService authenticationService;

        public DeleteUserFromRoomCommand(
            IConsole console,
            IUserService userService,
            IUserRoomService roomService,
            IAuthenticationService authenticationService,
            IRoomService room1Service)
            : base(console)
        {
            this.userService = userService;
            this.roomService = roomService;
            this.authenticationService = authenticationService;
            this.room1Service = room1Service;
            this.Console = console;
        }

        public override string Name => "/deleteUserFromRoom";

        public override string Description => " Deletе a user from room. You must be logged in.";

        public async override Task<OptionalResult<string>> Execute(string token)
        {
            var userResult = await this.authenticationService.GetUserByToken(token);

            if (!userResult.IsSuccess)
            {
                return new OptionalResult<string>(userResult);
            }

            var user = userResult.Value;

            var rooms = (await this.roomService.GetRoomsForUser(user)).ToList();
            var apply = this.OutputAvailableRooms(rooms);

            if (apply)
            {
                var roomIdResult = this.GetRoom(rooms);
                if (!roomIdResult.IsSuccess)
                {
                    return new OptionalResult<string>(roomIdResult);
                }

                var room = (await this.room1Service.GetByCondition(x => x.Id.Value == roomIdResult.Value)).FirstOrDefault();

                var users = (await this.roomService.GetUsersInRoom(room)).ToList();
                var apply1 = this.OutputAvailableUsers(users);

                if (apply1)
                {
                    var deleteUserId = this.GetUserToBeDeletedId(users);

                    var result = await this.roomService.DeleteUserFromRoom(deleteUserId.Result.Value, roomIdResult.Value);
                    if (result.IsSuccess)
                    {
                        this.Console.Print($"User {deleteUserId} deleted from room {roomIdResult.Value} successfully\n");
                    }
                }
            }

            return new OptionalResult<string>();
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

        private bool OutputAvailableUsers(List<UserModel> users)
        {
            var apply = true;
            this.Console.Print("Users in the room:\n");
            this.Console.Print("\n");

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                this.Console.Print($"{i + 1}) {user.UserName}\n");
            }

            this.Console.Print("\n");
            return apply;
        }

        private OptionalResult<int> GetRoom(List<RoomModel> rooms)
        {
            while (true)
            {
                int.TryParse(this.Console.Input("Which room would you like to delete a user from? "), out var number);
                if (rooms[number - 1].Id.IsSuccess)
                {
                    return rooms[number - 1].Id;
                }

                this.Console.Print("Enter a valid number.");
            }
        }

        private async Task<OptionalResult<int>> GetUserToBeDeletedId(List<UserModel> users)
        {
            while (true)
            {
                var email = this.Console.Input("What is the username of the user you would like to delete?: ");
                if (!string.IsNullOrWhiteSpace(email))
                {
                    var user = (await this.userService.GetByCondition(x => x.UserName == email)).FirstOrDefault();
                    if (user.Id.IsSuccess)
                    {
                        return user.Id;
                    }
                }

                this.Console.Print("Enter a valid username.");
            }
        }
    }
    }
