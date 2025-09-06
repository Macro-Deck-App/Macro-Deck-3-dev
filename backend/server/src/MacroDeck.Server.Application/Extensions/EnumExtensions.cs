using System.ComponentModel;
using System.Reflection;

namespace MacroDeck.Server.Application.Extensions;

public static class EnumExtensions
{
	public static string GetDescription(this Enum member)
	{
		var memberInfo = member.GetType().GetMember(member.ToString());

		if (memberInfo.Length > 0)
		{
			var attr = memberInfo[0].GetCustomAttribute<DescriptionAttribute>();
			if (attr != null)
			{
				return attr.Description;
			}
		}

		return member.ToString();
	}
}
