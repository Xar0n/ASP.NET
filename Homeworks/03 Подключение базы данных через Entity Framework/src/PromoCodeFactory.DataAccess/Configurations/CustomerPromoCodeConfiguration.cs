namespace PromoCodeFactory.DataAccess.Configurations;

public class CustomerPromoCodeConfiguration : IEntityTypeConfiguration<CustomerPromoCode>
{
    public void Configure(EntityTypeBuilder<CustomerPromoCode> builder)
    {
        builder.Property(x => x.CustomerId)
            .IsRequired(true);

        builder.Property(x => x.PromoCodeId)
            .IsRequired(true);

        builder.Property(x => x.CreatedAt)
            .IsRequired(true);
    }
}
