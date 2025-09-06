using MacroDeck.Server.Application.DataTypes;

namespace MacroDeck.Server.Application.Extensions;

public static class EnumerableExtensions
{
	public static IEnumerable<TreeItem<T>> ToTree<T, TParent>(
		this IEnumerable<T> collection,
		Func<T, TParent> idSelector,
		Func<T, TParent> parentIdSelector,
		TParent? rootId = default)
	{
		var enumerable = collection as T[] ?? collection.ToArray();
		foreach (var c in enumerable.Where(x => EqualityComparer<TParent>.Default.Equals(parentIdSelector(x), rootId)))
		{
			yield return new TreeItem<T>
						 {
							 Item = c,
							 Children = enumerable.ToTree(idSelector, parentIdSelector, idSelector(c))
						 };
		}
	}
}
