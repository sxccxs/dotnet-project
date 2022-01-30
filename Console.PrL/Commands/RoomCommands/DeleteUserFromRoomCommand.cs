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
        private readonly IUserRoomService roomService;
        private readonly IAuthenticationService authenticationService;

        public DeleteUserFromRoomCommand(
            IConsole console,
            IUserRoomService roomService,
            IAuthenticationService authenticationService)
            : base(console)
        {
            this.roomService = roomService;
            this.authenticationService = authenticationService;
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

            if (!apply)
            {
                return new OptionalResult<string>();
            }

            var room = this.GetSelectedItem(rooms);
            var users = (await this.roomService.GetUsersInRoom(room)).ToList();
            this.OutputAvailableUsers(users);

            var deleteUser = this.GetSelectedItem(users);

            var result = await this.roomService.DeleteUserFromRoom(deleteUser, room);
            if (result.IsSuccess)
            {
                this.Console.Print($"User {deleteUser.Id} deleted from room {room.Id} successfully\n");
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

        private void OutputAvailableUsers(List<UserModel> users)
        {
            this.Console.Print("Users in the room:\n");
            this.Console.Print("\n");

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                this.Console.Print($"{i + 1}) {user.UserName}\n");
            }

            this.Console.Print("\n");
        }

        private T GetSelectedItem<T>(List<T> rooms)
        {
            int index;
            while (true)
            {
                var parsed = int.TryParse(this.Console.Input("Enter number of item to select: "), out index);
                index--;
                if (!parsed)
                {
                    this.Console.Print("Enter a valid number.\n");
                }
                else if (index < 0 || index >= rooms.Count)
                {
                    this.Console.Print("Enter a number, that is in range.\n");
                }
                else
                {
                    break;
                }
            }

            return rooms[index];
        }
    }
}
