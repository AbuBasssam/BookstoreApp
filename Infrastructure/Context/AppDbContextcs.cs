using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure;
public class AppDbContext : IdentityDbContext<User, Role, int,
                            IdentityUserClaim<int>, UserRole,
                            IdentityUserLogin<int>, IdentityRoleClaim<int>,
                            IdentityUserToken<int>>
{
    public DbSet<Author> Authors { get; set; }
    public DbSet<Publisher> Publishers { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<BookRating> BookRatings { get; set; }
    public DbSet<Language> Languages { get; set; }
    public DbSet<BookCopy> BookCopies { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<BookView> BookView { get; set; }
    public DbSet<SystemSettings> Settings { get; set; }
    public DbSet<BorrowingRecord> BorrowingRecords { get; set; }
    public DbSet<BorrowingRecordView> BorrowingRecordsView { get; set; }
    public DbSet<ReservationRecordView> ReservationRecordsViews { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<UserDevice> UserDevices { get; set; }
    public DbSet<Otp> Otps { get; set; }

    public AppDbContext(DbContextOptions options) : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Remove the UserRole & IdentityUserToken tables
        modelBuilder.Ignore<UserRole>();
        modelBuilder.Ignore<IdentityUserToken<int>>();

        modelBuilder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
        modelBuilder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
        modelBuilder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
        modelBuilder.Entity<UserRefreshToken>().ToTable("UserRefreshTokens");



    }
}
