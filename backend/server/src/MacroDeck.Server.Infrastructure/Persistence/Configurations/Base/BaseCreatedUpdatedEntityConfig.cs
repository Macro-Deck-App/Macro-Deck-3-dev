using MacroDeck.Server.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroDeck.Server.Infrastructure.Persistence.Configurations.Base;

public class BaseCreatedUpdatedEntityConfig<T> : BaseCreatedEntityConfig<T>
	where T : BaseCreatedUpdatedEntity
{
	public BaseCreatedUpdatedEntityConfig(string table, string columnPrefix)
		: base(table, columnPrefix)
	{
	}

	public override void Configure(EntityTypeBuilder<T> builder)
	{
		base.Configure(builder);

		builder.Property(x => x.Updated)
			   .HasColumnName(ColumnPrefix + "update_timestamp");
	}
}
