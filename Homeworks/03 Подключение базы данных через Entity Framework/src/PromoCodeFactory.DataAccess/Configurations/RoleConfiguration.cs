namespace PromoCodeFactory.DataAccess.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired(true)
            .HasMaxLength(100);

        builder.Property(x => x.Description)
            .HasMaxLength(500);
    }
}
