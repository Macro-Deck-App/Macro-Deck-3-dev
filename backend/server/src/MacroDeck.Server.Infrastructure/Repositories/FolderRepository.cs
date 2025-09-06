using MacroDeck.Server.Application.Repositories;
using MacroDeck.Server.Domain.Entities;
using MacroDeck.Server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace MacroDeck.Server.Infrastructure.Repositories;

public class FolderRepository : IFolderRepository
{
	private readonly DatabaseContext _context;

	public FolderRepository(DatabaseContext context)
	{
		_context = context;
	}

	public ValueTask<FolderEntity?> GetFolderById(int id)
	{
		return _context.Folders.FindAsync(id);
	}

	public Task<int> CountChildFolders(int parentFolderRef)
	{
		return _context.Folders.AsNoTracking().Where(x => x.ParentFolderRef == parentFolderRef).CountAsync();
	}

	public async Task Create(FolderEntity folder)
	{
		await _context.Folders.AddAsync(folder);
		await _context.SaveChangesAsync();
	}

	public Task<bool> DisplayNameExists(string displayName, int? parentFolderId)
	{
		var baseQuery = _context.Folders.AsNoTracking().Where(x => x.DisplayName == displayName);

		if (parentFolderId.HasValue)
		{
			baseQuery = baseQuery.Where(x => x.ParentFolderRef == parentFolderId);
		}

		return baseQuery.AnyAsync();
	}

	public Task Save()
	{
		return _context.SaveChangesAsync();
	}

	public Task DeleteFolderByTree(string folderTree)
	{
		return _context.Folders.Where(x => x.Tree.StartsWith(folderTree)).ExecuteDeleteAsync();
	}

	public Task<List<FolderEntity>> GetFolders()
	{
		return _context.Folders.ToListAsync();
	}

	public Task<int> CountAllFolders()
	{
		return _context.Folders.AsNoTracking().CountAsync();
	}

	public Task<List<int>> GetFoldersIncludingChildFoldersByTree(string folderTree)
	{
		return _context.Folders.AsNoTracking()
					   .Where(x => x.Tree.StartsWith(folderTree))
					   .Select(x => x.Id)
					   .ToListAsync();
	}
}
