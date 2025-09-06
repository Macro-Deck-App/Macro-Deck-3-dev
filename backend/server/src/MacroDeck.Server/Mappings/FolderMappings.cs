using MacroDeck.Server.Application.DataTypes;
using MacroDeck.Server.Domain.Entities;
using MacroDeck.Server.Models.Folders;
using Riok.Mapperly.Abstractions;

namespace MacroDeck.Server.Mappings;

[Mapper]
public partial class FolderMapper
{
	[MapperIgnoreSource(nameof(FolderEntity.Widgets))]
	[MapperIgnoreSource(nameof(FolderEntity.ParentFolder))]
	[MapperIgnoreSource(nameof(FolderEntity.Created))]
	[MapperIgnoreSource(nameof(FolderEntity.Updated))]
	public partial FolderModel FolderEntityToFolderModel(FolderEntity folder);

	[MapProperty(nameof(TreeItem<FolderEntity>.Item.Id), nameof(FolderWithChildsModel.Id))]
	[MapProperty(nameof(TreeItem<FolderEntity>.Item.DisplayName), nameof(FolderWithChildsModel.DisplayName))]
	[MapProperty(nameof(TreeItem<FolderEntity>.Item.Index), nameof(FolderWithChildsModel.Index))]
	[MapProperty(nameof(TreeItem<FolderEntity>.Item.ParentFolderRef), nameof(FolderWithChildsModel.ParentFolderRef))]
	[MapProperty(nameof(TreeItem<FolderEntity>.Item.Tree), nameof(FolderWithChildsModel.Tree))]
	[MapProperty(nameof(TreeItem<FolderEntity>.Item.Rows), nameof(FolderWithChildsModel.Rows))]
	[MapProperty(nameof(TreeItem<FolderEntity>.Item.Columns), nameof(FolderWithChildsModel.Columns))]
	[MapProperty(nameof(TreeItem<FolderEntity>.Item.BackgroundColor), nameof(FolderWithChildsModel.BackgroundColor))]
	[MapProperty(nameof(TreeItem<FolderEntity>.Children), nameof(FolderWithChildsModel.Childs))]
	public partial FolderWithChildsModel FolderTreeItemToFolderWithChildsModel(TreeItem<FolderEntity> folderTreeItem);

	public partial IEnumerable<FolderWithChildsModel> FolderTreeItemListToFolderWithChildsModelList(
		IEnumerable<TreeItem<FolderEntity>> folders);

	[MapperIgnoreTarget(nameof(FolderEntity.Widgets))]
	[MapperIgnoreTarget(nameof(FolderEntity.ParentFolder))]
	[MapperIgnoreTarget(nameof(FolderEntity.Created))]
	[MapperIgnoreTarget(nameof(FolderEntity.Updated))]
	public partial FolderEntity FolderModelToFolderEntity(FolderModel model);
}
