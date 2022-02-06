using Console.PrL.Commands;
using Console.PrL.Commands.RoomCommands;
using Console.PrL.Commands.UserCommands;
using Console.PrL.Interfaces;
using Console.PrL.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Console.PrL;

public static class DependencyRegistrar
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        BLL.DependencyRegistrar.ConfigureServices(services, configuration);
        services.AddScoped<IConsole, CustomConsole>();
        services.AddScoped<App>();
        ConfigureUserCommands(services);
        ConfigureRoomCommands(services);
    }

    private static void ConfigureUserCommands(IServiceCollection services)
    {
        services.AddScoped<Command, LoginCommand>();
        services.AddScoped<Command, RegistrationCommand>();
        services.AddScoped<Command, ActivationCommand>();
        services.AddScoped<Command, MeCommand>();
        services.AddScoped<Command, EditAccountCommand>();
        services.AddScoped<Command, DeleteAccountCommand>();
    }

    private static void ConfigureRoomCommands(IServiceCollection services)
    {
        services.AddScoped<Command, GetRoomsCommand>();
        services.AddScoped<Command, CreateRoomCommand>();
        services.AddScoped<Command, UpdateRoomCommand>();
        services.AddScoped<Command, DeleteRoomCommand>();
        services.AddScoped<Command, DeleteUserFromRoomCommand>();
        services.AddScoped<Command, AddUserToRoomCommand>();
        services.AddScoped<Command, ChangeRoleCommand>();
    }
}