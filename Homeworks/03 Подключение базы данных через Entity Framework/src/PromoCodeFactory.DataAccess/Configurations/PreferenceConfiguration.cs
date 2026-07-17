namespace PromoCodeFactory.DataAccess.Configurations;

public class PreferenceConfiguration : IEntityTypeConfiguration<Preference>
{
    public void Configure(EntityTypeBuilder<Preference> builder)
    {
        builder.Property(x => x.Name)
            .IsRequired(true)
            .HasMaxLength(100);
    }
}
