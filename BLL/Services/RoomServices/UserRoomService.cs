using BLL.Abstractions.Interfaces.RoomInterfaces;
using BLL.Abstractions.Interfaces.UserInterfaces;
using Core.DataClasses;
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

        public ExceptionalResult CreateRoomForUser(UserModel user, RoomCreateModel createModel)
        {
            var validationResult = this.validationService.ValidateCreateModel(createModel);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            createModel.Users = new List<int> { user.Id };

            var roomResult = this.roomService.Create(createModel);
            if (!roomResult.IsSuccess)
            {
                return roomResult;
            }

            var room = roomResult.Value;
            user.Rooms.Add(room.Id);

            var userUpdateData = new UserUpdateModel()
            {
                Id = user.Id,
                Rooms = user.Rooms,
            };

            var updateResult = this.userService.Update(userUpdateData);
            if (!updateResult.IsSuccess)
            {
                return updateResult;
            }

            return new ExceptionalResult();
        }

        public ExceptionalResult DeleteRoomByUser(UserModel user, int roomId)
        {
            var room = this.roomService.GetByCondition(x => x.Id == roomId).FirstOrDefault();
            if (room is null)
            {
                return new ExceptionalResult(false, $"Room with id {roomId} does not exist");
            }

            if (!room.Users.Contains(user.Id))
            {
                return new ExceptionalResult(false, $"User with id {user.Id} does not belong to room with id {room.Id}");
            }

            foreach (var roomUser in this.GetUsersInRoom(room))
            {
                var updateModel = new UserUpdateModel()
                {
                    Id = roomUser.Id,
                    Rooms = roomUser.Rooms.Where(x => x != room.Id).ToList(),
                };

                var updateResult = this.userService.Update(updateModel);

                if (!updateResult.IsSuccess)
                {
                    return updateResult;
                }
            }

            var deleteResult = this.roomService.Delete(room.Id);
            if (!deleteResult.IsSuccess)
            {
                return deleteResult;
            }

            return new ExceptionalResult();
        }

        public ExceptionalResult UpdateRoomForUser(UserModel user, RoomUpdateModel updateModel)
        {
            var room = this.roomService.GetByCondition(x => x.Id == updateModel.Id).FirstOrDefault();
            if (room is null)
            {
                return new ExceptionalResult(false, $"Room with id {updateModel.Id} does not exist");
            }

            if (!room.Users.Contains(user.Id))
            {
                return new ExceptionalResult(false, $"User with id {user.Id} does not belong to room with id {room.Id}");
            }

            var validationResult = this.validationService.ValidateUpdateModel(updateModel);

            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            var updateResult = this.roomService.Update(updateModel);

            if (!updateResult.IsSuccess)
            {
                return updateResult;
            }

            return new ExceptionalResult();
        }
    }
}
