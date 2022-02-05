﻿using Core.Models;
using Core.Models.RoomModels;
using Core.Models.UserModels;
using Microsoft.EntityFrameworkCore;

namespace DAL;

public class AppContext : DbContext
{
    public AppContext()
    {
    }

    public AppContext(DbContextOptions<AppContext> options)
        : base(options)
    {
    }

    public DbSet<UserModel> Users { get; set; }

    public DbSet<RoomModel> Rooms { get; set; }

    public DbSet<RoleTypeModel> RoleTypes { get; set; }

    public DbSet<RoleModel> Roles { get; set; }
}