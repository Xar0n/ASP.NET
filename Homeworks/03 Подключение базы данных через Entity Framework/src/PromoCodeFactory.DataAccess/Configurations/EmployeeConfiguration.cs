namespace PromoCodeFactory.DataAccess.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
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

        builder.HasOne(x => x.Role)
            .WithMany()
            .HasForeignKey("RoleId");
    }
}
