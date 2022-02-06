﻿using Core.DataClasses;
using Core.Models.RoomModels;

namespace BLL.Abstractions.Interfaces.RoomInterfaces
{
    public interface IRoomService
    {
        Task<IEnumerable<RoomModel>> GetAll();

        Task<IEnumerable<RoomModel>> GetByCondition(Func<RoomModel, bool> condition);

        Task<OptionalResult<RoomModel>> Create(RoomCreateModel roomModel);

        Task<OptionalResult<RoomModel>> Update(RoomUpdateModel roomModel);

        Task<OptionalResult<RoomModel>> Delete(int id);
    }
}
