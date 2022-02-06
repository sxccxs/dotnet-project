﻿using BLL.Abstractions.Interfaces.RoomInterfaces;
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

        public override string Description => "Deletes a user from room. You must be logged in.";

        public override async Task<OptionalResult<string>> Execute(string token)
        {
            var userResult = await this.authenticationService.GetUserByToken(token);

            if (!userResult.IsSuccess)
            {
                return new OptionalResult<string>(userResult);
            }

            var user = userResult.Value;

            var rooms = user.Rooms.ToHashSet().ToList();
            var apply = this.OutputAvailableRooms(rooms);

            if (!apply)
            {
                return new OptionalResult<string>();
            }

            var room = this.GetSelectedItem(rooms);
            var users = room.Users.ToHashSet().ToList();
            this.OutputAvailableUsers(users);

            var deleteUser = this.GetSelectedItem(users);

            var result = await this.roomService.DeleteUserFromRoom(deleteUser, room);
            if (!result.IsSuccess)
            {
                return new OptionalResult<string>(result);
            }

            this.Console.Print($"User {deleteUser.Id} deleted from room {room.Id} successfully");

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

        private void OutputAvailableUsers(List<UserModel> users)
        {
            this.Console.Print("Users in the room:");
            this.Console.Print();

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                this.Console.Print($"{i + 1}) {user.UserName} {user.Email}");
            }

            this.Console.Print();
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
                    this.Console.Print("Enter a valid number.");
                }
                else if (index < 0 || index >= rooms.Count)
                {
                    this.Console.Print("Enter a number, that is in range.");
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
