using AutoMapper;
using BLL.Abstractions.Interfaces.RoomInterfaces;
using Core.Exceptions;
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

        public IEnumerable<RoomModel> GetAll()
        {
            return this.storage.GetAll();
        }

        public IEnumerable<RoomModel> GetByCondition(Func<RoomModel, bool> condition)
        {
            return this.storage.GetByCondition(condition);
        }

        public RoomModel Create(RoomCreateModel roomModel)
        {
            var room = this.MapRoomCreateModelToRoomModel(roomModel);
            this.storage.Create(room);

            return room;
        }

        public RoomModel Delete(int id)
        {
            var room = this.storage.GetByCondition(x => x.Id == id).FirstOrDefault();
            if (room is null)
            {
                throw new RoomDoesNotExistException($"Room with id {id} does not exist");
            }

            this.storage.Delete(room);

            return room;
        }

        public RoomModel Update(RoomUpdateModel roomModel)
        {
            if (!this.storage.GetByCondition(x => x.Id == roomModel.Id).Any())
            {
                throw new RoomDoesNotExistException($"Room with id {roomModel.Id} does not exist");
            }

            var room = this.MapRoomUpdateModelToRoomModel(roomModel);
            this.storage.Update(room);

            return room;
        }

        private RoomModel MapRoomCreateModelToRoomModel(RoomCreateModel createModel)
        {
            var mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap<RoomCreateModel, RoomModel>());
            var mapper = new Mapper(mapperConfiguration);
            var room = mapper.Map<RoomModel>(createModel);
            room.Id = this.storage.GetNextId();
            Console.WriteLine(room.Id);

            foreach (var prop in room.GetType().GetProperties())
            {
                if (prop.GetValue(room) is null && prop.PropertyType == typeof(List<int>))
                {
                    prop.SetValue(room, new List<int>());
                }
            }

            return room;
        }

        private RoomModel MapRoomUpdateModelToRoomModel(RoomUpdateModel updateModel)
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<RoomUpdateModel, RoomModel>().ForAllMembers(opt => opt.AllowNull());
            });
            var mapper = new Mapper(mapperConfiguration);
            var room = mapper.Map<RoomModel>(updateModel);
            var changingRoom = this.storage.GetByCondition(x => x.Id == updateModel.Id).First();

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
