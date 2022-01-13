using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.Exceptions;
using Core.Models.RoomModels;
using Core.Models.UserModels;

namespace BLL.Services.RoomServices
{
    internal class UserRoomService : IUserRoomService
    {
        private readonly IRoomService roomService;

        private readonly IUserService userService;

        private readonly IRoomValidationService validationService;

        public UserRoomService(IRoomService roomService, IUserService userService, IRoomValidationService validationService)
        {
            this.roomService = roomService;
            this.userService = userService;
            this.validationService = validationService;
        }

        public IEnumerable<RoomModel> GetRoomsForUser(UserModel user)
        {
            return user.Rooms.Select(id => this.roomService.GetByCondition(x => x.Id == id).First());
        }

        public IEnumerable<UserModel> GetUsersInRoom(RoomModel room)
        {
            return room.Users.Select(id => this.userService.GetByCondition(x => x.Id == id).First());
        }

        public void CreateRoomForUser(UserModel user, RoomCreateModel createModel)
        {
            this.validationService.ValidateCreateModel(createModel);
            createModel.Users = new List<int> { user.Id };

            var room = this.roomService.Create(createModel);
            user.Rooms.Add(room.Id);

            var userUpdateData = new UserUpdateModel()
            {
                Id = user.Id,
                Rooms = user.Rooms,
            };

            this.userService.Update(userUpdateData);
        }

        public void DeleteRoomByUser(UserModel user, int roomId)
        {
            var room = this.roomService.GetByCondition(x => x.Id == roomId).FirstOrDefault();
            if (room is null)
            {
                throw new RoomDoesNotExistException($"Room with id {roomId} does not exist");
            }

            if (!room.Users.Contains(user.Id))
            {
                throw new UserDoesNotBelongToRoomException($"User with id {user.Id} does not belong to room with id {room.Id}");
            }

            foreach (var roomUser in this.GetUsersInRoom(room))
            {
                var updateModel = new UserUpdateModel()
                {
                    Id = roomUser.Id,
                    Rooms = roomUser.Rooms.Where(x => x != room.Id).ToList(),
                };
                this.userService.Update(updateModel);
            }

            this.roomService.Delete(room.Id);
        }

        public void UpdateRoomForUser(UserModel user, RoomUpdateModel updateModel)
        {
            var room = this.roomService.GetByCondition(x => x.Id == updateModel.Id).FirstOrDefault();
            if (room is null)
            {
                throw new RoomDoesNotExistException($"Room with id {updateModel.Id} does not exist");
            }

            if (!room.Users.Contains(user.Id))
            {
                throw new UserDoesNotBelongToRoomException($"User with id {user.Id} does not belong to room with id {room.Id}");
            }

            this.validationService.ValidateUpdateModel(updateModel);

            this.roomService.Update(updateModel);
        }
    }
}
