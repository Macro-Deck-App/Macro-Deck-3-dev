using MacroDeck.Server.Domain.Entities;
using MacroDeck.Server.Models.Widgets;
using Riok.Mapperly.Abstractions;

namespace MacroDeck.Server.Mappings;

[Mapper]
public partial class WidgetMapper
{
	[MapperIgnoreSource(nameof(WidgetEntity.FolderRef))]
	[MapperIgnoreSource(nameof(WidgetEntity.Folder))]
	[MapperIgnoreSource(nameof(WidgetEntity.Created))]
	[MapperIgnoreSource(nameof(WidgetEntity.Updated))]
	public partial WidgetModel WidgetEntityToWidgetModel(WidgetEntity widgetEntity);
	
	public partial IEnumerable<WidgetModel> WidgetEntityListToWidgetModelList(IList<WidgetEntity> widgetEntities);
}
