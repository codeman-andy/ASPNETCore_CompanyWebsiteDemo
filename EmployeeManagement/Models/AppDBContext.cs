using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Models
{
    // IdentityDbContext uses the IdentityUser-class by default, and so we must notify it of
    // our custom ApplicationUser-class in order to properly construct the Context.
    // Furthermore, the migrations apparently derive from this Context-object the different
    // models they need to keep track of, and so until we tell IdentityDbContext of our custom
    // user-class, the columns for our custom user-class would not show up in the underlying DB.
    public class AppDBContext : IdentityDbContext<ApplicationUser>
    {
        // DbContextOptions<SomeDbContext the options are going to apply to>
        public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
        {
            // The DBContextOptions-instance carries the configuration information
            // such as the connection-string to use, the database-provider to use, etc.
        }

        // This property queries and saves instances of the Employee class
        // The LINQ-queries written to this DbSet will be translated into SQL-queries
        public DbSet<Employee> Employees { get; set; }
        // The name of the DbSet-object (in this case, Employees) will be used as the
        // name for the table in the database by EFCore


        protected override void OnModelCreating(ModelBuilder model_builder)
        {
            base.OnModelCreating(model_builder);
            model_builder.Seed();

            // This sets the foreign-key referential integrity constraint to prevent the deletion of foreign keys on the database
            // which have children that point to it, as opposed to deleting all the children once you delete a key they point to
            // In our application, this works specifically so that when we try to delete a role that has users associated to it
            // we will get an error that impedes us from deleting that role until we unassign all users in that role
            foreach (var foreign_key in model_builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                foreign_key.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
