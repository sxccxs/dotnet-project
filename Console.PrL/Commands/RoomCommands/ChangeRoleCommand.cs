using BLL.Abstractions.Interfaces.RoleInterfaces;
using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Console.PrL.Interfaces;
using Core.DataClasses;
using Core.Enums;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace Console.PrL.Commands.RoomCommands
{
    internal class ChangeRoleCommand : Command
    {
        private readonly IAuthenticationService authenticationService;

        private readonly IUserRoomService userRoomService;

        private readonly IUserRoomRoleService roleService;

        public ChangeRoleCommand(
            IConsole console,
            IAuthenticationService authenticationService,
            IUserRoomService userRoomService,
            IUserRoomRoleService roleService)
            : base(console)
        {
            this.authenticationService = authenticationService;
            this.userRoomService = userRoomService;
            this.roleService = roleService;
        }

        public override string Name => "/changeRoles";

        public override string Description => "Changes role for selected user in room";

        public async override Task<OptionalResult<string>> Execute(string token)
        {
            var userResult = await this.authenticationService.GetUserByToken(token);
            if (!userResult.IsSuccess)
            {
                return new OptionalResult<string>(userResult);
            }

            var user = userResult.Value;
            var rooms = (await this.userRoomService.GetRoomsForUser(user)).ToList();
            var isContinue = this.ListRooms(rooms);
            if (!isContinue)
            {
                return new OptionalResult<string>();
            }

            var room = this.GetSelectedItem(rooms);
            if ((await this.roleService.GetRoleForUserAndRoom(user, room)).Role != Role.ADMIN)
            {
                return new OptionalResult<string>(false, $"You dont have rights to chage roles in room {room.Name}");
            }

            var users = (await this.userRoomService.GetUsersInRoom(room)).ToList();
            await this.ListUsers(users, room);
            var editUser = this.GetSelectedItem(users);
            this.ListRoles(editUser);
            var role = this.GetSelectedItem(Enum.GetValues(typeof(Role)).Cast<Role>().ToList());
            var updateResult = await this.roleService.UpdateRoleForUser(editUser, room, role);
            if (!updateResult.IsSuccess)
            {
                return new OptionalResult<string>(updateResult);
            }

            this.Console.Print("Role changes successfuly");

            return new OptionalResult<string>();
        }

        private bool ListRooms(List<RoomModel> rooms)
        {
            this.Console.Print("Select room:");
            this.Console.Print();
            if (rooms.Count == 0)
            {
                this.Console.Print("<It's empty here...>");
            }

            for (int i = 0; i < rooms.Count; i++)
            {
                var room = rooms[i];
                this.Console.Print($"{i + 1}) {room.Name}");
            }

            this.Console.Print();

            return rooms.Count != 0;
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

        private async Task ListUsers(List<UserModel> users, RoomModel room)
        {
            this.Console.Print("Select room:");
            this.Console.Print();

            for (int i = 0; i < users.Count; i++)
            {
                var user = users[i];
                var role = await this.roleService.GetRoleForUserAndRoom(user, room);
                this.Console.Print($"{i + 1}) {user.UserName} {user.Email} {role.Role}");
            }

            this.Console.Print();
        }

        private void ListRoles(UserModel user)
        {
            this.Console.Print($"Select new role for user {user.UserName}:");
            var values = Enum.GetValues(typeof(Role)).Cast<Role>().ToList();
            for (var i = 0; i < values.Count(); i++)
            {
                var role = values[i];
                this.Console.Print($"{i + 1}) {role}");
            }
        }
    }
}
