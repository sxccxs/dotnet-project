using System.Linq.Expressions;
using AutoMapper;
using BLL.Abstractions.Interfaces.RoomInterfaces;
using Core.DataClasses;
using Core.Models.RoomModels;
using DAL.Abstractions.Interfaces;

namespace BLL.Services.RoomServices
{
    internal class RoomService : IRoomService
    {
        private readonly IGenericStorageWorker<RoomModel> storage;

        public RoomService(IGenericStorageWorker<RoomModel> storage)
        {
            this.storage = storage;
        }

        public async Task<IEnumerable<RoomModel>> GetByConditions(params Expression<Func<RoomModel, bool>>[] conditions)
        {
            return await this.storage.GetByConditions(conditions, r => r.Roles, r => r.Users);
        }

        public async Task<RoomModel> GetRoomById(int id)
        {
            return (await this.GetByConditions(r => r.Id == id)).FirstOrDefault();
        }

        public async Task<OptionalResult<RoomModel>> Create(RoomCreateModel roomModel)
        {
            var room = this.MapRoomCreateModelToRoomModel(roomModel);
            await this.storage.Create(room);

            return new OptionalResult<RoomModel>(room);
        }

        public async Task<ExceptionalResult> Delete(int id)
        {
            var room = await this.GetRoomById(id);
            if (room is null)
            {
                return new ExceptionalResult(false, $"Room with id {id} does not exist");
            }

            await this.storage.Delete(room);

            return new ExceptionalResult();
        }

        public async Task<OptionalResult<RoomModel>> Update(RoomUpdateModel roomModel)
        {
            if (await this.GetRoomById(roomModel.Id) is null)
            {
                return new OptionalResult<RoomModel>(false, $"Room with id {roomModel.Id} does not exist");
            }

            var room = await this.MapRoomUpdateModelToRoomModel(roomModel);
            await this.storage.Update(room);

            return new OptionalResult<RoomModel>(room);
        }

        private RoomModel MapRoomCreateModelToRoomModel(RoomCreateModel createModel)
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap<RoomCreateModel, RoomModel>());
            var mapper = new Mapper(mapperConfiguration);
            var room = mapper.Map<RoomModel>(createModel);

            return room;
        }

        private async Task<RoomModel> MapRoomUpdateModelToRoomModel(RoomUpdateModel updateModel)
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RoomUpdateModel, RoomModel>().ForAllMembers(opt => opt.AllowNull());
            });
            var mapper = new Mapper(mapperConfiguration);
            var room = mapper.Map<RoomModel>(updateModel);
            var changingRoom = await this.GetRoomById(updateModel.Id);

            foreach (var prop in room.GetType().GetProperties())
            {
                if (prop.GetValue(room) is not null)
                {
                    prop.SetValue(changingRoom, prop.GetValue(room));
                }
            }

            return changingRoom;
        }
    }
}
