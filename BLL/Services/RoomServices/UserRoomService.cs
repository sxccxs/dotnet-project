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

        public async Task<IEnumerable<RoomModel>> GetRoomsForUser(UserModel user)
        {
            var tasks = await Task.WhenAll(user.Rooms.Select(id => this.roomService.GetByCondition(x => id == user.Id)));
            return tasks.Where(result => result is not null).Select(result => result.First());
        }

        public async Task<IEnumerable<UserModel>> GetUsersInRoom(RoomModel room)
        {
            var tasks = await Task.WhenAll(room.Users.Select(id => this.userService.GetByCondition(x => x.Id == id)));
            return tasks.Where(result => result is not null).Select(result => result.First());
        }

        public async Task<ExceptionalResult> CreateRoomForUser(UserModel user, RoomCreateModel createModel)
        {
            var validationResult = this.validationService.ValidateCreateModel(createModel);
            if (!validationResult.IsSuccess)
            {
                return validationResult;
            }

            createModel.Users = new List<int> { user.Id };

            var roomResult = await this.roomService.Create(createModel);
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

            var updateResult = await this.userService.Update(userUpdateData);
            if (!updateResult.IsSuccess)
            {
                return updateResult;
            }

            return new ExceptionalResult();
        }

        public async Task<ExceptionalResult> DeleteRoomByUser(UserModel user, int roomId)
        {
            var room = (await this.roomService.GetByCondition(x => x.Id == roomId)).FirstOrDefault();
            if (room is null)
            {
                return new ExceptionalResult(false, $"Room with id {roomId} does not exist");
            }

            if (!room.Users.Contains(user.Id))
            {
                return new ExceptionalResult(false, $"User with id {user.Id} does not belong to room with id {room.Id}");
            }

            foreach (var roomUser in await this.GetUsersInRoom(room))
            {
                var updateModel = new UserUpdateModel()
                {
                    Id = roomUser.Id,
                    Rooms = roomUser.Rooms.Where(x => x != room.Id).ToList(),
                };

                var updateResult = await this.userService.Update(updateModel);

                if (!updateResult.IsSuccess)
                {
                    return updateResult;
                }
            }

            var deleteResult = await this.roomService.Delete(room.Id);
            if (!deleteResult.IsSuccess)
            {
                return deleteResult;
            }

            return new ExceptionalResult();
        }

        public async Task<ExceptionalResult> UpdateRoomForUser(UserModel user, RoomUpdateModel updateModel)
        {
            var room = (await this.roomService.GetByCondition(x => x.Id == updateModel.Id)).FirstOrDefault();
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

            var updateResult = await this.roomService.Update(updateModel);

            if (!updateResult.IsSuccess)
            {
                return updateResult;
            }

            return new ExceptionalResult();
        }
    }
}
