using Microsoft.EntityFrameworkCore;

namespace ElementaryMathStudyWebsite.Repositories.Context
{
    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
    }
}
