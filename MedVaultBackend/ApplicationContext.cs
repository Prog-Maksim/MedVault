using MedVaultBackend.Models;
using MedVaultBackend.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace MedVaultBackend;

public class ApplicationContext: DbContext
{
    public DbSet<Users> Users { get; set; } = null!;
    public DbSet<Documents> Documents { get; set; } = null!;
    public DbSet<MedicalInstitution> MedicalInstitution { get; set; } = null!;
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options): base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Настройка связи "один ко многим"
        modelBuilder.Entity<Documents>()
            .HasOne(d => d.User) // Один документ связан с одним пользователем
            .WithMany(u => u.Documents) // У одного пользователя много документов
            .HasForeignKey(d => d.PersonId) // Внешний ключ в таблице Documents
            .HasPrincipalKey(u => u.PersonId) // Основной ключ в таблице Users
            .OnDelete(DeleteBehavior.Cascade); // Каскадное удаление, если требуется
        
        // Настройка связи "много к одному" между Documents и MedicalInstitution
        modelBuilder.Entity<Documents>()
            .HasOne(d => d.MedicalInstitution) // Один документ связан с одной медицинской организацией
            .WithMany(m => m.Documents)       // У одной медицинской организации много документов
            .HasForeignKey(d => d.Address)     // Внешний ключ в таблице Documents
            .HasPrincipalKey(m => m.Id)       // Основной ключ в таблице MedicalInstitution
            .OnDelete(DeleteBehavior.SetNull); // Установить null при удалении MedicalInstitution
    }
}