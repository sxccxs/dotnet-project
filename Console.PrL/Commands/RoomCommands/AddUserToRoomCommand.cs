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

        public AddUserToRoomCommand(
            IConsole console,
            IAuthenticationService authenticationService,
            IUserRoomService userRoomService)
            : base(console)
        {
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
            if (!apply)
            {
                return new OptionalResult<string>();
            }

            var room = this.GetRoom(rooms);

            var users = (await this.userRoomService.GetUsersInRoom(room)).ToList();

            var email = this.Console.Input("What is the e-mail of the user you would like to add?: ");
            var result = await this.userRoomService.AddUserToRoom(email, room);
            if (!result.IsSuccess)
            {
                return new OptionalResult<string>(result);
            }

            this.Console.Print($"User with email {email} added successfully");

            return new OptionalResult<string>();
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

        private RoomModel GetRoom(List<RoomModel> rooms)
        {
            int number;
            while (true)
            {
                var parsed = int.TryParse(
                    this.Console.Input("Which room would you like to add a user to? "), out number);
                number--;
                if (!parsed)
                {
                    this.Console.Print("Enter a valid number");
                }
                else if (number < 0 || number > rooms.Count)
                {
                    this.Console.Print("Enter valid in valid range.");
                }
                else
                {
                    break;
                }
            }

            return rooms[number];
        }
    }
}
