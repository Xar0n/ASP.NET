namespace PromoCodeFactory.DataAccess;

public class PromoCodeFactoryDbContext : DbContext
{
    public DbSet<Employee> Employees { get; set; }

    public DbSet<Role> Roles { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Preference> Preferences { get; set; }

    public DbSet<PromoCode> PromoCodes { get; set; }

    public DbSet<CustomerPromoCode> CustomerPromoCodes { get; set; }

    public PromoCodeFactoryDbContext(DbContextOptions<PromoCodeFactoryDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PromoCodeFactoryDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
