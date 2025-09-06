using MacroDeck.Server.Domain.Entities;
using MacroDeck.Server.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroDeck.Server.Infrastructure.Persistence.Configurations;

public class IntegrationConfigurationEntityConfig : BaseCreatedUpdatedEntityConfig<IntegrationConfigurationEntity>
{
	public IntegrationConfigurationEntityConfig()
		: base("integration_configuration", "ic_")
	{
	}

	public override void Configure(EntityTypeBuilder<IntegrationConfigurationEntity> builder)
	{
		base.Configure(builder);

		builder.Property(x => x.IntegrationId)
			   .HasColumnName(ColumnPrefix + "integration_identifier")
			   .IsRequired();

		builder.Property(x => x.ConfigurationKey)
			   .HasColumnName(ColumnPrefix + "key")
			   .IsRequired();

		builder.Property(x => x.EncryptedConfigurationValue)
			   .HasColumnName(ColumnPrefix + "value")
			   .HasColumnType("BLOB")
			   .IsRequired();

		builder.Property(x => x.CanBeAccessFromOtherIntegrations)
			   .HasColumnName(ColumnPrefix + "public")
			   .HasDefaultValue(false);
	}
}
