using MacroDeck.Server.Domain.Entities;

namespace MacroDeck.Server.Application.Repositories;

public interface IFolderRepository
{
	ValueTask<FolderEntity?> GetFolderById(int id);

	Task<int> CountChildFolders(int parentFolderRef);

	Task Create(FolderEntity folder);

	Task<bool> DisplayNameExists(string displayName, int? parentFolderId);

	Task Save();

	Task DeleteFolderByTree(string folderTree);

	Task<List<FolderEntity>> GetFolders();

	Task<int> CountAllFolders();

	Task<List<int>> GetFoldersIncludingChildFoldersByTree(string folderTree);
}
