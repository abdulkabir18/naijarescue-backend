using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Persistence.Context
{
    public class ProjectDbContextFactory : IDesignTimeDbContextFactory<ProjectDbContext>
    {
        public ProjectDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProjectDbContext>();

            optionsBuilder.UseMySql("server=localhost;database=NaijaResuceSystemDb;user=root;password=Pa$$word",
                    ServerVersion.AutoDetect("server=localhost;database=NaijaResuceSystemDb;user=root;password=Pa$$word")); 

            return new ProjectDbContext(optionsBuilder.Options);
        }
    }
}
