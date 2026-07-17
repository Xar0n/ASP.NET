namespace PromoCodeFactory.DataAccess.Configurations;

public class PromoCodeConfiguration : IEntityTypeConfiguration<PromoCode>
{
    public void Configure(EntityTypeBuilder<PromoCode> builder)
    {
        builder.Property(x => x.Code)
            .IsRequired(true)
            .HasMaxLength(100);

        builder.Property(x => x.ServiceInfo)
            .IsRequired(true)
            .HasMaxLength(256);

        builder.Property(x => x.PartnerName)
            .IsRequired(true)
            .HasMaxLength(256);

        builder.Property(x => x.BeginDate)
            .IsRequired(true);

        builder.Property(x => x.EndDate)
            .IsRequired(true);

        builder.HasOne(x => x.PartnerManager)
            .WithMany()
            .HasForeignKey("PartnerManagerId");

        builder.HasOne(x => x.Preference)
            .WithMany()
            .HasForeignKey("PreferenceId");

        builder.HasMany(x => x.CustomerPromoCodes)
            .WithOne()
            .HasForeignKey(x => x.PromoCodeId);
    }
}
