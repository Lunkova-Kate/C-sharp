using Microsoft.EntityFrameworkCore;

namespace MasterOfSkipping;

public class AppOnDBContext : DbContext
{
    public AppOnDBContext(DbContextOptions<AppOnDBContext> options) : base(options)
    {
    }

    public DbSet<SubjectEntity> Subjects => Set<SubjectEntity>();
}