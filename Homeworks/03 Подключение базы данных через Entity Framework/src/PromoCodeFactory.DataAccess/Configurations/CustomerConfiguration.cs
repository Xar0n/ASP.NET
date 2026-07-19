namespace PromoCodeFactory.DataAccess.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.Property(x => x.FirstName)
            .IsRequired(true)
            .HasMaxLength(50);

        builder.Property(x => x.LastName)
            .IsRequired(true)
            .HasMaxLength(50);

        builder.Property(x => x.Email)
            .IsRequired(true)
            .HasMaxLength(256);

        builder.Ignore(x => x.FullName);

        builder.HasMany(x => x.Preferences)
            .WithMany(x => x.Customers);

        builder.HasMany(x => x.CustomerPromoCodes)
            .WithOne()
            .HasForeignKey(x => x.CustomerId);
    }
}
