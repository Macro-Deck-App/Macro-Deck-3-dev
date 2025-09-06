namespace MacroDeck.Server.Domain.Entities.Base;

public class BaseCreatedUpdatedEntity : BaseCreatedEntity
{
	public DateTime? Updated { get; set; }
}
