using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;
using Core.Models.RoomModels;

namespace Console.PrL.Commands.RoomCommands
{
    internal class DeleteRoomCommand : Command
    {
        private readonly IUserRoomService roomService;

        private readonly IAuthenticationService authenticationService;

        public DeleteRoomCommand(IConsole console, IAuthenticationService authenticationService, IUserRoomService roomService)
            : base(console)
        {
            this.roomService = roomService;
            this.authenticationService = authenticationService;
        }

        public override string Name => "/deleteRoom";

        public async override Task<OptionalResult<string>> Execute(string token)
        {
            var userResult = await this.authenticationService.GetUserByToken(token);

            if (!userResult.IsSuccess)
            {
                return new OptionalResult<string>(userResult);
            }

            var user = userResult.Value;

            var rooms = (await this.roomService.GetRoomsForUser(user)).ToList();

            int deletionId;
            var apply = this.GetDeleteRoomId(rooms, out deletionId);

            if (apply)
            {
                var deleteResult = await this.roomService.DeleteRoomByUser(user, deletionId);

                if (!deleteResult.IsSuccess)
                {
                    return new OptionalResult<string>(deleteResult);
                }

                this.Console.Print("Room deleted successfully\n");
            }

            return new OptionalResult<string>();
        }

        private bool GetDeleteRoomId(List<RoomModel> rooms, out int deletionId)
        {
            var apply = this.OutputAvailableRooms(rooms);

            deletionId = this.GetDeletionId(rooms);

            return apply;
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

        private int GetDeletionId(List<RoomModel> rooms)
        {
            int index;
            while (true)
            {
                var parsed = int.TryParse(this.Console.Input("Enter number of room to delete: "), out index);
                index--;
                if (!parsed)
                {
                    this.Console.Print("Enter a valid number.");
                }
                else if (index < 0 || index >= rooms.Count)
                {
                    this.Console.Print("Enter a number, that is in range of rooms.");
                }
                else
                {
                    break;
                }
            }

            return rooms[index].Id;
        }
    }
}
