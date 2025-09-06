using MacroDeck.Server.Domain.Entities;

namespace MacroDeck.Server.Application.Services;

public interface IFolderService
{
	Task<FolderEntity> CreateFolder(
		string displayName,
		int? parentFolderRef);

	Task<FolderEntity> GetFolderById(int id);

	Task UpdateFolder(int id, FolderEntity folderUpdate);

	Task DeleteFolder(int id);

	Task<List<FolderEntity>> GetFolders();

	Task EnsureDefaultFolderExists();
}
