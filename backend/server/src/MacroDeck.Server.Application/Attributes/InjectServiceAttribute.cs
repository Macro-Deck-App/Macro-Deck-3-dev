using MacroDeck.Server.Domain.Enums;

namespace MacroDeck.Server.Application.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class InjectServiceAttribute : Attribute
{

	public InjectServiceAttribute(ServiceLifetime lifetime, Type? serviceType = null)
	{
		Lifetime = lifetime;
		ServiceType = serviceType;
	}

	public ServiceLifetime Lifetime { get; set; }

	public Type? ServiceType { get; set; }
}
