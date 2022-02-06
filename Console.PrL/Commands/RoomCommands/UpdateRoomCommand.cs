using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;
using Core.Models.RoomModels;

namespace Console.PrL.Commands.RoomCommands
{
    internal class UpdateRoomCommand : Command
    {
        private readonly IUserRoomService roomService;

        private readonly IAuthenticationService authenticationService;

        public UpdateRoomCommand(IConsole console, IAuthenticationService authenticationService, IUserRoomService roomService)
            : base(console)
        {
            this.roomService = roomService;
            this.authenticationService = authenticationService;
        }

        public override string Name => "/editRoom";

        public override string Description => "Edit room info. You must be logged";

        public async override Task<OptionalResult<string>> Execute(string token)
        {
            var userResult = await this.authenticationService.GetUserByToken(token);
            if (!userResult.IsSuccess)
            {
                return new OptionalResult<string>(userResult);
            }

            var user = userResult.Value;

            var rooms = (await this.roomService.GetRoomsForUser(user)).ToList();

            var roomUpdateData = new RoomEditModel();

            var apply = this.GetRoomUpdateData(rooms, roomUpdateData);

            if (apply)
            {
                var result = await this.roomService.UpdateRoomForUser(user, roomUpdateData);
                if (!result.IsSuccess)
                {
                    return new OptionalResult<string>(result);
                }

                this.Console.Print("Room updated successfully");
            }

            return new OptionalResult<string>();
        }

        private bool GetRoomUpdateData(List<RoomModel> rooms, RoomEditModel roomUpdateData)
        {
            var apply = this.OutputAvailableRooms(rooms);

            this.SetEditingRoomId(rooms, roomUpdateData);
            this.SetNewName(roomUpdateData);
            if (roomUpdateData.Name is null)
            {
                roomUpdateData.Name = rooms.Where(x => x.Id == roomUpdateData.Id).First().Name;
            }

            return apply;
        }

        private bool OutputAvailableRooms(List<RoomModel> rooms)
        {
            var apply = true;
            this.Console.Print("Rooms you are in:");
            this.Console.Print();
            if (rooms.Count == 0)
            {
                this.Console.Print("<You don't have any rooms to edit>");
                apply = false;
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                var room = rooms[i];
                this.Console.Print($"{i + 1}) {room.Name}");
            }

            this.Console.Print();
            return apply;
        }

        private void SetEditingRoomId(List<RoomModel> rooms, RoomEditModel roomUpdateData)
        {
            int index;
            while (true)
            {
                var parsed = int.TryParse(this.Console.Input("Enter number of room to edit: "), out index);
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

            roomUpdateData.Id = rooms[index].Id;
        }

        private void SetNewName(RoomEditModel roomUpdateData)
        {
            var newName = this.Console.Input("New Name(leave blank to not change): ");

            if (!string.IsNullOrWhiteSpace(newName))
            {
                roomUpdateData.Name = newName;
            }
        }
    }
}
