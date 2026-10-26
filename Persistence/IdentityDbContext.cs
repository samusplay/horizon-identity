using Horizon.Identity.Domain;
using Microsoft.EntityFrameworkCore;

//Conexion a la base de Datos de Sql Server
public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    //Definimos la Tabla de User En La Base de datos
    public DbSet<User> Users => Set<User>();

    // Tokens de recuperación de contraseña (un solo uso).
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(u => u.Email).HasMaxLength(256).IsRequired();
            entity.Property(u => u.PasswordHash).HasMaxLength(256).IsRequired();

            // Evita dos cuentas con el mismo correo (a nivel de base de datos).
            entity.HasIndex(u => u.Email).IsUnique();
        });

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            entity.Property(t => t.TokenHash).HasMaxLength(64).IsRequired();

            entity.HasIndex(t => t.TokenHash).IsUnique();
            entity.HasIndex(t => t.UserId);

            entity.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
