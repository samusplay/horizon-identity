using Horizon.Identity.Domain;
using Microsoft.EntityFrameworkCore;

//Conexion a la base de Datos de Sql Server
public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options) { }

    //Definimos la Tabla de User En La Base de datos
    public DbSet<User> Users => Set<User>();
}