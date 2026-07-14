using Microsoft.EntityFrameworkCore;
using TourApi.Models;

namespace TourApi.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<Country> Countries => Set<Country>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<HotelService> HotelServices => Set<HotelService>();
    public DbSet<Hotel> Hotels => Set<Hotel>();
    public DbSet<HotelImage> HotelImages => Set<HotelImage>();
    public DbSet<HotelServiceMapping> HotelServiceMappings => Set<HotelServiceMapping>();
    public DbSet<Tour> Tours => Set<Tour>();
    public DbSet<TourDetail> TourDetails => Set<TourDetail>();
    public DbSet<TourImage> TourImages => Set<TourImage>();
    public DbSet<User> Users => Set<User>();
    public DbSet<ExternalLogin> ExternalLogins => Set<ExternalLogin>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<EmailConfirmationToken> EmailConfirmationTokens => Set<EmailConfirmationToken>();
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Tourist> Tourists => Set<Tourist>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<TouringHistory> TouringHistories => Set<TouringHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ---------- User ----------
        modelBuilder.Entity<User>(e =>
        {
            e.ToTable("User");
            e.HasKey(x => x.Id).HasName("PK_User");
            e.Property(x => x.Username).HasMaxLength(30).IsRequired();
            e.Property(x => x.PasswordHash).HasColumnType("binary(64)").IsRequired();
            e.Property(x => x.EmailConfirmed).HasDefaultValue(false);
            e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.HasIndex(x => x.Username).IsUnique().HasDatabaseName("IX_User_Username");
        });

        // ---------- ExternalLogin ----------
        modelBuilder.Entity<ExternalLogin>(e =>
        {
            e.ToTable("ExternalLogins");
            e.HasKey(x => x.Id).HasName("PK_ExternalLogins");
            e.Property(x => x.Provider).HasMaxLength(32).IsRequired();
            e.Property(x => x.ProviderUserId).HasMaxLength(256).IsRequired();
            e.Property(x => x.Email).HasMaxLength(256);
            e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);

            e.HasOne(x => x.User)
                .WithMany(x => x.ExternalLogins)
                .HasForeignKey(x => x.UserId)
                .HasConstraintName("FK_ExternalLogins_User_UserId")
                .OnDelete(DeleteBehavior.Cascade);

            e.HasIndex(x => x.UserId).HasDatabaseName("IX_ExternalLogins_UserId");
            e.HasIndex(x => new { x.Provider, x.ProviderUserId })
                .IsUnique()
                .HasDatabaseName("IX_ExternalLogins_Provider_ProviderUserId");
        });

        // ---------- RefreshToken ----------
        modelBuilder.Entity<RefreshToken>(e =>
        {
            e.ToTable("RefreshTokens");
            e.HasKey(x => x.Id).HasName("PK_RefreshTokens");
            e.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
            e.Property(x => x.CreatedByIp).HasMaxLength(64);
            e.Property(x => x.RevokedByIp).HasMaxLength(64);
            e.Property(x => x.ReplacedByTokenHash).HasMaxLength(128);
            e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .HasConstraintName("FK_RefreshTokens_User_UserId")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.TokenHash).IsUnique().HasDatabaseName("IX_RefreshTokens_TokenHash");
            e.HasIndex(x => x.UserId).HasDatabaseName("IX_RefreshTokens_UserId");
        });

        // ---------- EmailConfirmationToken ----------
        modelBuilder.Entity<EmailConfirmationToken>(e =>
        {
            e.ToTable("EmailConfirmationTokens");
            e.HasKey(x => x.Id).HasName("PK_EmailConfirmationTokens");
            e.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
            e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.HasOne(x => x.User)
                .WithMany(x => x.EmailConfirmationTokens)
                .HasForeignKey(x => x.UserId)
                .HasConstraintName("FK_EmailConfirmationTokens_User_UserId")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.TokenHash).IsUnique().HasDatabaseName("IX_EmailConfirmationTokens_TokenHash");
            e.HasIndex(x => x.UserId).HasDatabaseName("IX_EmailConfirmationTokens_UserId");
        });

        // ---------- PasswordResetToken ----------
        modelBuilder.Entity<PasswordResetToken>(e =>
        {
            e.ToTable("PasswordResetTokens");
            e.HasKey(x => x.Id).HasName("PK_PasswordResetTokens");
            e.Property(x => x.TokenHash).HasMaxLength(128).IsRequired();
            e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.HasOne(x => x.User)
                .WithMany(x => x.PasswordResetTokens)
                .HasForeignKey(x => x.UserId)
                .HasConstraintName("FK_PasswordResetTokens_User_UserId")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.TokenHash).IsUnique().HasDatabaseName("IX_PasswordResetTokens_TokenHash");
            e.HasIndex(x => x.UserId).HasDatabaseName("IX_PasswordResetTokens_UserId");
        });

        // ---------- Country ----------
        modelBuilder.Entity<Country>(e =>
        {
            e.ToTable("Countries");
            e.HasKey(x => x.Id).HasName("PK_Countries");
            e.Property(x => x.Name).HasMaxLength(30).IsRequired();
            e.Property(x => x.IsoName).HasMaxLength(2).IsRequired();
            e.Property(x => x.FlagUrl).HasMaxLength(500);
            e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.HasIndex(x => x.IsoName).IsUnique().HasDatabaseName("IX_Countries_IsoName");
            e.HasIndex(x => x.Name).IsUnique().HasDatabaseName("IX_Countries_Name");
        });

        // ---------- City ----------
        modelBuilder.Entity<City>(e =>
        {
            e.ToTable("Cities");
            e.HasKey(x => x.Id).HasName("PK_Cities");
            e.Property(x => x.Name).HasMaxLength(50).IsRequired();
            e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.HasOne(x => x.Country)
                .WithMany(x => x.Cities)
                .HasForeignKey(x => x.CountryId)
                .HasConstraintName("FK_Cities_Countries_CountryId")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.CountryId).HasDatabaseName("IX_Cities_CountryId");
        });

        // ---------- HotelService ----------
        modelBuilder.Entity<HotelService>(e =>
        {
            e.ToTable("HotelServices");
            e.HasKey(x => x.Id).HasName("PK_HotelServices");
            e.Property(x => x.Name).HasMaxLength(30).IsRequired();
        });

        // ---------- Hotel ----------
        modelBuilder.Entity<Hotel>(e =>
        {
            e.ToTable("Hotels");
            e.HasKey(x => x.Id).HasName("PK_Hotels");
            e.Property(x => x.Name).HasMaxLength(50).IsRequired();
            e.Property(x => x.StarRating).HasColumnType("tinyint");
            e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.HasOne(x => x.City)
                .WithMany(x => x.Hotels)
                .HasForeignKey(x => x.CityId)
                .HasConstraintName("FK_Hotels_Cities_CityId")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.CityId).HasDatabaseName("IX_Hotels_CityId");
        });

        // ---------- HotelImage ----------
        modelBuilder.Entity<HotelImage>(e =>
        {
            e.ToTable("HotelImages");
            e.HasKey(x => x.Id).HasName("PK_HotelImages");
            e.Property(x => x.Url).HasMaxLength(500).IsRequired();
            e.Property(x => x.FileName).HasMaxLength(255).IsRequired();
            e.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
            e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.HasOne(x => x.Hotel)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.HotelId)
                .HasConstraintName("FK_HotelImages_Hotels_HotelId")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.HotelId).HasDatabaseName("IX_HotelImages_HotelId");
        });

        // ---------- HotelServiceMapping (composite PK, join table) ----------
        modelBuilder.Entity<HotelServiceMapping>(e =>
        {
            e.ToTable("HotelServiceMappings");
            e.HasKey(x => new { x.HotelId, x.HotelServiceId }).HasName("PK_HotelServiceMappings");
            e.HasOne(x => x.Hotel)
                .WithMany(x => x.HotelServiceMappings)
                .HasForeignKey(x => x.HotelId)
                .HasConstraintName("FK_HotelServiceMappings_Hotels_HotelId")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(x => x.HotelService)
                .WithMany(x => x.HotelServiceMappings)
                .HasForeignKey(x => x.HotelServiceId)
                .HasConstraintName("FK_HotelServiceMappings_HotelServices_HotelServiceId")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.HotelServiceId).HasDatabaseName("IX_HotelServiceMappings_HotelServiceId");
        });

        // ---------- Tour ----------
        modelBuilder.Entity<Tour>(e =>
        {
            e.ToTable("Tours");
            e.HasKey(x => x.Id).HasName("PK_Tours");
            e.Property(x => x.Code).HasMaxLength(50).IsRequired();
            e.Property(x => x.Name).HasColumnType("nvarchar(max)").IsRequired();
            e.Property(x => x.Description).HasMaxLength(1000);
            e.Property(x => x.CurrentPrice).HasColumnType("money");
            e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.HasIndex(x => x.Code).IsUnique().HasDatabaseName("IX_Tours_Code");
            e.HasOne(x => x.CreatedByEmployee)
                .WithMany()
                .HasForeignKey(x => x.CreatedByEmployeeId)
                .HasConstraintName("FK_Tours_Employees_CreatedByEmployeeId")
                .OnDelete(DeleteBehavior.NoAction);
            e.HasIndex(x => x.CreatedByEmployeeId).HasDatabaseName("IX_Tours_CreatedByEmployeeId");

            e.HasOne(x => x.AssignedTourGuide)
                .WithMany()
                .HasForeignKey(x => x.AssignedTourGuideId)
                .HasConstraintName("FK_Tours_Employees_AssignedTourGuideId")
                .OnDelete(DeleteBehavior.NoAction);
            e.HasIndex(x => x.AssignedTourGuideId).HasDatabaseName("IX_Tours_AssignedTourGuideId");
            
            e.HasOne(x => x.AssignedTravelAgent)
                .WithMany()
                .HasForeignKey(x => x.AssignedTravelAgentId)
                .HasConstraintName("FK_Tours_Employees_AssignedTravelAgentId")
                .OnDelete(DeleteBehavior.NoAction);

            e.HasIndex(x => x.AssignedTravelAgentId)
                .HasDatabaseName("IX_Tours_AssignedTravelAgentId");
        });

        // ---------- TourImage ----------
        modelBuilder.Entity<TourImage>(e =>
        {
            e.ToTable("TourImages");
            e.HasKey(x => x.Id).HasName("PK_TourImages");
            e.Property(x => x.Url).HasMaxLength(500).IsRequired();
            e.Property(x => x.FileName).HasMaxLength(255).IsRequired();
            e.Property(x => x.ContentType).HasMaxLength(100).IsRequired();
            e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.HasOne(x => x.Tour)
                .WithMany(x => x.Images)
                .HasForeignKey(x => x.TourId)
                .HasConstraintName("FK_TourImages_Tours_TourId")
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(x => x.TourId).HasDatabaseName("IX_TourImages_TourId");
        });

        // ---------- TourDetail ----------
        modelBuilder.Entity<TourDetail>(e =>
        {
            e.ToTable("TourDetails");
            e.HasKey(x => x.Id).HasName("PK_TourDetails");
            e.Property(x => x.Sequence).HasColumnType("tinyint");
            e.Property(x => x.EstimatedArrivalDate).HasColumnType("datetime2(0)");
            e.Property(x => x.EstimatedDepartureDate).HasColumnType("datetime2(0)");
            e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);

            e.HasOne(x => x.Tour)
                .WithMany(x => x.TourDetails)
                .HasForeignKey(x => x.TourId)
                .HasConstraintName("FK_TourDetails_Tours_TourId")
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.City)
                .WithMany(x => x.TourDetails)
                .HasForeignKey(x => x.CityId)
                .HasConstraintName("FK_TourDetails_Cities_CityId")
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Hotel)
                .WithMany(x => x.TourDetails)
                .HasForeignKey(x => x.HotelId)
                .HasConstraintName("FK_TourDetails_Hotels_HotelId")
                .OnDelete(DeleteBehavior.NoAction);

            e.HasIndex(x => x.CityId).HasDatabaseName("IX_TourDetails_CityId");
            e.HasIndex(x => x.HotelId).HasDatabaseName("IX_TourDetails_HotelId");
            e.HasIndex(x => x.TourId).HasDatabaseName("IX_TourDetails_TourId");
        });

        // ---------- Employee (TPH: Employee -> TravelAgent / TourGuide) ----------
        modelBuilder.Entity<Employee>(e =>
        {
            e.ToTable("Employees");
            e.HasKey(x => x.Id).HasName("PK_Employees");
            e.Property(x => x.ContactPhone).HasMaxLength(20).IsRequired();
            e.Property(x => x.Email).HasMaxLength(50).IsRequired();
            e.Property(x => x.FirstName).HasMaxLength(50).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(50).IsRequired();
            e.Property(x => x.NationalId).HasMaxLength(20).IsRequired();
            e.Property(x => x.Experience).HasMaxLength(200);
            e.Property(x => x.Gender).HasMaxLength(1).IsRequired();
            e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);

            e.HasOne(x => x.User)
                .WithOne(x => x.Employee)
                .HasForeignKey<Employee>(x => x.UserId)
                .HasConstraintName("FK_Employees_User_UserId")
                .OnDelete(DeleteBehavior.NoAction);

            e.HasIndex(x => x.UserId).IsUnique().HasDatabaseName("IX_Employees_UserId");

            e.HasDiscriminator<string>("Discriminator")
                .HasValue<TravelAgent>(nameof(TravelAgent))
                .HasValue<TourGuide>(nameof(TourGuide))
                .HasValue<Admin>(nameof(Admin));

            e.Property<string>("Discriminator").HasMaxLength(13);

            e.ToTable(t => t.HasCheckConstraint("CK_PersonalInfo_Gender", "[Gender] = 'F' OR [Gender] = 'M'"));
        });

        // ---------- Tourist ----------
        modelBuilder.Entity<Tourist>(e =>
        {
            e.ToTable("Tourists", t => t.HasCheckConstraint("CK_PersonalInfo_Gender1", "[Gender] = 'F' OR [Gender] = 'M'"));
            e.HasKey(x => x.Id).HasName("PK_Tourists");
            e.Property(x => x.ContactPhone).HasMaxLength(20).IsRequired();
            e.Property(x => x.Email).HasMaxLength(50).IsRequired();
            e.Property(x => x.FirstName).HasMaxLength(50).IsRequired();
            e.Property(x => x.LastName).HasMaxLength(50).IsRequired();
            e.Property(x => x.NationalId).HasMaxLength(20).IsRequired();
            e.Property(x => x.Gender).HasMaxLength(1).IsRequired();
            e.Property(x => x.CreateDate).HasDefaultValueSql("getdate()");
            e.Property(x => x.IsDeleted).HasDefaultValue(false);

            e.HasOne(x => x.User)
                .WithOne(x => x.Tourist)
                .HasForeignKey<Tourist>(x => x.UserId)
                .HasConstraintName("FK_Tourists_User_UserId")
                .OnDelete(DeleteBehavior.NoAction);

            e.HasIndex(x => x.UserId).IsUnique().HasDatabaseName("IX_Tourists_UserId");
        });

        // ---------- Booking ----------
        modelBuilder.Entity<Booking>(e =>
        {
            e.ToTable("Bookings");
            e.HasKey(x => x.Id).HasName("PK_Bookings");
            e.Property(x => x.PricePaid).HasColumnType("money");
            e.Property(x => x.Status).HasConversion<int>();

            e.HasOne(x => x.Tour)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.TourId)
                .HasConstraintName("FK_Bookings_Tours_TourId")
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Tourist)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.TouristId)
                .HasConstraintName("FK_Bookings_Tourists_TouristId")
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.TravelAgent)
                .WithMany(x => x.Bookings)
                .HasForeignKey(x => x.TravelAgentId)
                .HasConstraintName("FK_Bookings_Employees_TravelAgentId")
                .OnDelete(DeleteBehavior.ClientCascade);

            e.HasIndex(x => x.TourId).HasDatabaseName("IX_Bookings_TourId");
            e.HasIndex(x => x.TouristId).HasDatabaseName("IX_Bookings_TouristId");
            e.HasIndex(x => x.TravelAgentId).HasDatabaseName("IX_Bookings_TravelAgentId");
        });

        // ---------- TouringHistory (composite PK, join table) ----------
        modelBuilder.Entity<TouringHistory>(e =>
        {
            e.ToTable("TouringHistories");
            e.HasKey(x => new { x.TouristId, x.TourId }).HasName("PK_TouringHistories");
            e.Property(x => x.DepartureDate).HasColumnType("datetime2(0)");
            e.Property(x => x.ReturnDate).HasColumnType("datetime2(0)");

            e.HasOne(x => x.Tourist)
                .WithMany(x => x.TouringHistories)
                .HasForeignKey(x => x.TouristId)
                .HasConstraintName("FK_TouringHistories_Tourists_TouristId")
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(x => x.Tour)
                .WithMany(x => x.TouringHistories)
                .HasForeignKey(x => x.TourId)
                .HasConstraintName("FK_TouringHistories_Tours_TourId")
                .OnDelete(DeleteBehavior.ClientCascade);

            e.HasIndex(x => x.TourId).HasDatabaseName("IX_TouringHistories_TourId");
        });
    }
}
