using MacroDeck.Server.Domain.Entities.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroDeck.Server.Infrastructure.Persistence.Configurations.Base;

public class BaseCreatedEntityConfig<T> : IEntityTypeConfiguration<T>
	where T : BaseCreatedEntity
{

	public BaseCreatedEntityConfig(string table, string columnPrefix)
	{
		Table = table;
		ColumnPrefix = columnPrefix;
	}

	public string Table { get; }

	public string ColumnPrefix { get; }

	public virtual void Configure(EntityTypeBuilder<T> builder)
	{
		builder.ToTable(Table);

		builder.Property(x => x.Id)
			   .HasColumnName(ColumnPrefix + "id");

		builder.Property(x => x.Created)
			   .HasColumnName(ColumnPrefix + "create_timestamp");
	}
}
