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

        public async Task<IEnumerable<RoomModel>> GetByCondition(Func<RoomModel, bool> condition)
        {
            return await this.storage.GetByCondition(condition);
        }

        public async Task<OptionalResult<RoomModel>> Create(RoomCreateModel roomModel)
        {
            var room = await this.MapRoomCreateModelToRoomModel(roomModel);
            await this.storage.Create(room);

            return new OptionalResult<RoomModel>(room);
        }

        public async Task<OptionalResult<RoomModel>> Delete(int id)
        {
            var room = (await this.storage.GetByCondition(x => x.Id == id)).FirstOrDefault();
            if (room is null)
            {
                return new OptionalResult<RoomModel>(false, $"Room with id {id} does not exist");
            }

            await this.storage.Delete(room);

            return new OptionalResult<RoomModel>(room);
        }

        public async Task<OptionalResult<RoomModel>> Update(RoomUpdateModel roomModel)
        {
            if (!(await this.storage.GetByCondition(x => x.Id == roomModel.Id)).Any())
            {
                return new OptionalResult<RoomModel>(false, $"Room with id {roomModel.Id} does not exist");
            }

            var room = await this.MapRoomUpdateModelToRoomModel(roomModel);
            await this.storage.Update(room);

            return new OptionalResult<RoomModel>(room);
        }

        private async Task<RoomModel> MapRoomCreateModelToRoomModel(RoomCreateModel createModel)
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap<RoomCreateModel, RoomModel>());
            var mapper = new Mapper(mapperConfiguration);
            var room = mapper.Map<RoomModel>(createModel);
            room.Id = await this.storage.GetNextId();

            foreach (var prop in room.GetType().GetProperties())
            {
                if (prop.GetValue(room) is null && prop.PropertyType == typeof(List<int>))
                {
                    prop.SetValue(room, new List<int>());
                }
            }

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
            var changingRoom = (await this.storage.GetByCondition(x => x.Id == updateModel.Id)).First();

            foreach (var prop in room.GetType().GetProperties())
            {
                if (prop.GetValue(room) is null)
                {
                    prop.SetValue(room, prop.GetValue(changingRoom));
                }
            }

            return room;
        }
    }
}
