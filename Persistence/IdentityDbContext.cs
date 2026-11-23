using Horizon.Identity.Domain;
using Microsoft.EntityFrameworkCore;

//Conexion a la base de Datos de Sql Server
public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    //Definimos la Tabla de User En La Base de datos
    public DbSet<User> Users => Set<User>();

    //Tokens de recuperación de contraseña (un solo uso)
    public DbSet<PasswordResetToken> PasswordResetTokens => Set<PasswordResetToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PasswordResetToken>(entity =>
        {
            //SHA-256 en hexadecimal siempre mide 64 caracteres
            entity.Property(t => t.TokenHash).HasMaxLength(64).IsRequired();

            //al restablecer se busca por el hash: índice para que sea rápido, y único
            entity.HasIndex(t => t.TokenHash).IsUnique();

            //para encontrar (e invalidar) los tokens de un usuario
            entity.HasIndex(t => t.UserId);

            //cada token pertenece a un usuario; si se borra el usuario, se borran sus tokens
            entity.HasOne(t => t.User)
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}