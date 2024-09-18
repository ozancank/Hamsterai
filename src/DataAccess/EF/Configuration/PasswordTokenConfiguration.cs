namespace DataAccess.EF.Configuration;

public class PasswordTokenConfiguration : IEntityTypeConfiguration<PasswordToken>
{
    public void Configure(EntityTypeBuilder<PasswordToken> builder)
    {
        builder.ToTable("PasswordTokens");
        builder.Property(e => e.Id).HasColumnName("Id").ValueGeneratedNever().HasColumnOrder(0).IsRequired();
        builder.HasKey(e => e.Id);
        builder.Property(e => e.CreateDate).HasColumnName("CreateDate").HasColumnOrder(1).IsRequired();
        builder.Property(e => e.UserId).HasColumnName("UserId").HasColumnOrder(2).IsRequired();
        builder.Property(e => e.Token).HasColumnName("Token").HasMaxLength(100).HasColumnOrder(3).IsRequired();
        builder.HasIndex(e => new { e.Token }).IsUnique();
        builder.Property(e => e.Expried).HasColumnName("Expried").HasColumnOrder(4).IsRequired();

        builder.HasOne(d => d.User).WithMany(p => p.PasswordTokens).HasForeignKey(d => d.UserId).HasPrincipalKey(x => x.Id);
    }
}