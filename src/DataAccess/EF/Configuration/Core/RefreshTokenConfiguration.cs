using Domain.Entities.Core;

namespace DataAccess.EF.Configuration.Core;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.ToTable("RefreshTokens");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.Token).HasColumnName("Token").HasMaxLength(100).HasColumnOrder(2).IsRequired();
        builder.Property(e => e.Expires).HasColumnName("Expires").HasColumnOrder(3).IsRequired();
        builder.Property(e => e.Created).HasColumnName("Created").HasColumnOrder(4).IsRequired();
        builder.Property(e => e.CreatedByIp).HasColumnName("CreatedByIp").HasMaxLength(50).HasColumnOrder(5).IsRequired();
        builder.Property(e => e.Revoked).HasColumnName("Revoked").HasColumnOrder(6);
        builder.Property(e => e.RevokedByIp).HasColumnName("RevokedByIp").HasMaxLength(50).HasColumnOrder(7);
        builder.Property(e => e.ReplacedByToken).HasColumnName("ReplacedByToken").HasMaxLength(100).HasColumnOrder(8);
        builder.Property(e => e.ReasonRevoked).HasColumnName("ReasonRevoked").HasMaxLength(100).HasColumnOrder(9);

        builder.HasOne(e => e.User).WithMany(e => e.RefreshTokens).HasForeignKey(e => e.UserId).HasPrincipalKey(e => e.Id);
    }
}