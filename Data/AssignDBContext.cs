using Assignment_1.Model;
using Microsoft.EntityFrameworkCore;

namespace Assignment_1.Data
{
    public class AssignDBContext: DbContext
    {
        public AssignDBContext() { }
        public AssignDBContext(DbContextOptions<AssignDBContext> options) : base(options)
        {
            
        }

        public DbSet<Emplyoee> Employee { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=LAKKI;Initial Catalog=assignment1;Integrated Security=True");
            }
        }
    }
}
