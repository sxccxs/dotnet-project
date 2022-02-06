namespace Core.Settings
{
    public class JsonDbSettings
    {
        // property name must be the same as model name: UserModel - UserDirectory
        public string UserDirectory { get; set; }

        public string RoomDirectory { get; set; }
    }
}
