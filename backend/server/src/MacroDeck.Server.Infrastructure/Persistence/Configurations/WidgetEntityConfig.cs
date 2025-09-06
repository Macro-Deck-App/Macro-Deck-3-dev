using MacroDeck.Server.Domain.Entities;
using MacroDeck.Server.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroDeck.Server.Infrastructure.Persistence.Configurations;

public class WidgetEntityConfig : BaseCreatedUpdatedEntityConfig<WidgetEntity>
{
	public WidgetEntityConfig()
		: base("widget", "w_")
	{
	}

	public override void Configure(EntityTypeBuilder<WidgetEntity> builder)
	{
		base.Configure(builder);

		builder.Property(x => x.FolderRef)
			   .HasColumnName(ColumnPrefix + "folder_ref")
			   .IsRequired();

		builder.Property(x => x.Type)
			   .HasColumnName(ColumnPrefix + "type")
			   .IsRequired();

		builder.Property(x => x.Row)
			   .HasColumnName(ColumnPrefix + "row")
			   .IsRequired();

		builder.Property(x => x.RowSpan)
			   .HasColumnName(ColumnPrefix + "row_span")
			   .IsRequired();

		builder.Property(x => x.CanIncreaseRowSpan)
			   .HasColumnName(ColumnPrefix + "increase_row_span_possible");

		builder.Property(x => x.CanDecreaseRowSpan)
			   .HasColumnName(ColumnPrefix + "decrease_row_span_possible");

		builder.Property(x => x.Column)
			   .HasColumnName(ColumnPrefix + "column")
			   .IsRequired();

		builder.Property(x => x.ColSpan)
			   .HasColumnName(ColumnPrefix + "col_span")
			   .IsRequired();

		builder.Property(x => x.CanIncreaseColSpan)
			   .HasColumnName(ColumnPrefix + "increase_col_span_possible");

		builder.Property(x => x.CanDecreaseColSpan)
			   .HasColumnName(ColumnPrefix + "decrease_col_span_possible");

		builder.Property(x => x.Data)
			   .HasColumnName(ColumnPrefix + "data");

		builder.Navigation(x => x.Folder)
			   .AutoInclude();

		builder.HasOne(x => x.Folder)
			   .WithMany(x => x.Widgets)
			   .HasForeignKey(x => x.FolderRef)
			   .OnDelete(DeleteBehavior.Cascade)
			   .IsRequired();
	}
}
