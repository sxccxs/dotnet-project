using Core.Models.ChatModels;
using Core.Models.MessagesModels;
using Core.Models.RoleModels;
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

    public DbSet<TextChatModel> TextChats { get; set; }

    public DbSet<VoiceChatModel> VoiceChats { get; set; }

    public DbSet<MessageModel> Messages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("data source=DESKTOP-D7HHFTG;initial catalog=projdb;trusted_connection=true");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<UserModel>()
            .HasIndex(u => u.Email)
            .IsUnique();
        builder.Entity<MessageModel>()
            .Property(m => m.SendingTime)
            .HasDefaultValueSql("getdate()");
        builder.Entity<TextChatModel>()
            .HasIndex(tc => new { tc.Name, tc.RoomId })
            .IsUnique();
        builder.Entity<VoiceChatModel>()
            .HasIndex(vc => new { vc.Name, vc.RoomId })
            .IsUnique();
        builder.Entity<MessageModel>()
            .HasOne(m => m.Author)
            .WithMany(u => u.Messages);
        builder.Entity<MessageModel>()
            .HasOne(m => m.ForwardedFrom);
    }
}