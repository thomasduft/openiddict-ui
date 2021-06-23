using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using tomware.OpenIddict.UI.Core;

namespace tomware.OpenIddict.UI.Infrastructure.Configuration
{
  public class ClaimTypeEntityConfiguration : IEntityTypeConfiguration<ClaimType>
  {
    public void Configure(EntityTypeBuilder<ClaimType> builder)
    {
      // table
      builder.ToTable("ClaimType");

      // columns
      builder.HasKey(x => x.Id);
      builder.HasIndex(x => x.Name).IsUnique();
      builder.Property(x => x.Name).IsRequired();
      builder.Property(x => x.ClaimValueType).IsRequired();
    }
  }
}