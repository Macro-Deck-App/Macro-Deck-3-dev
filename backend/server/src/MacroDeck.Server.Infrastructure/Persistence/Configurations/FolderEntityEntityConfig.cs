using MacroDeck.Server.Domain.Entities;
using MacroDeck.Server.Infrastructure.Persistence.Configurations.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MacroDeck.Server.Infrastructure.Persistence.Configurations;

public class FolderEntityEntityConfig : BaseCreatedUpdatedEntityConfig<FolderEntity>
{
	public FolderEntityEntityConfig()
		: base("folder", "f_")
	{
	}

	public override void Configure(EntityTypeBuilder<FolderEntity> builder)
	{
		base.Configure(builder);

		builder.Property(x => x.DisplayName)
			   .HasColumnName(ColumnPrefix + "display_name")
			   .IsRequired();

		builder.Property(x => x.Index)
			   .HasColumnName(ColumnPrefix + "index")
			   .IsRequired();

		builder.Property(x => x.ParentFolderRef)
			   .HasColumnName(ColumnPrefix + "parent_folder_ref");

		builder.Property(x => x.Tree)
			   .HasColumnName(ColumnPrefix + "tree")
			   .IsRequired();

		builder.Property(x => x.Rows)
			   .HasColumnName(ColumnPrefix + "row_count")
			   .IsRequired();

		builder.Property(x => x.Columns)
			   .HasColumnName(ColumnPrefix + "column_count")
			   .IsRequired();

		builder.Property(x => x.BackgroundColor)
			   .HasColumnName(ColumnPrefix + "background_color");

		builder.Navigation(x => x.Widgets).AutoInclude();

		builder.HasOne(x => x.ParentFolder)
			   .WithOne()
			   .HasForeignKey<FolderEntity>(x => x.ParentFolderRef)
			   .OnDelete(DeleteBehavior.Cascade)
			   .IsRequired(false);
	}
}
