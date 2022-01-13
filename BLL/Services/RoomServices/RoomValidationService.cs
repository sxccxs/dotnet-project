using BLL.Abstractions.Interfaces.RoomInterfaces;
using Core.Exceptions;
using Core.Models.RoomModels;

namespace BLL.Services.RoomServices
{
    internal class RoomValidationService : IRoomValidationService
    {
        private int MinNameLength => 8;

        private int MaxNameLength => 24;

        public void ValidateCreateModel(RoomCreateModel createModel)
        {
            this.ValidateName(createModel.Name);
        }

        public void ValidateUpdateModel(RoomUpdateModel updateModel)
        {
            this.ValidateName(updateModel.Name);
        }

        private void ValidateName(string name)
        {
            if (name is null || name.Length < this.MinNameLength)
            {
                throw new ValidationException($"Room name can't be less then {this.MinNameLength} symbols");
            }

            if (name.Length > this.MaxNameLength)
            {
                throw new ValidationException($"Room name can't be longer then {this.MaxNameLength} symbols");
            }
        }
    }
}
