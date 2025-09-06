using MacroDeck.Server.Application.Exceptions;
using MacroDeck.Server.Application.Notifications.Folders;
using MacroDeck.Server.Application.Repositories;
using MacroDeck.Server.Domain.Entities;
using MacroDeck.Server.Domain.Enums;
using Serilog;

namespace MacroDeck.Server.Application.Services.Implementations;

public class FolderService : IFolderService
{
	private readonly IFolderRepository _folderRepository;
	private readonly ILogger _logger;
	private readonly IPublisherService _publisherService;

	public FolderService(
		IFolderRepository folderRepository,
		IPublisherService publisherService,
		ILogger logger)
	{
		_folderRepository = folderRepository;
		_publisherService = publisherService;
		_logger = logger;
	}

	public async Task<FolderEntity> CreateFolder(
		string displayName,
		int? parentFolderRef = null)
	{
		await ValidateDisplayName(displayName, parentFolderRef);

		var folder = new FolderEntity
					 {
						 DisplayName = displayName,
						 Index = 0,
						 ParentFolderRef = parentFolderRef,
						 Tree = string.Empty,
						 Rows = 3,
						 Columns = 5
					 };

		FolderEntity? parentFolder = null;

		if (parentFolderRef.HasValue)
		{
			parentFolder = await _folderRepository.GetFolderById(parentFolderRef.Value);
			if (parentFolder is null)
			{
				throw new ErrorCodeException(ErrorCode.ParentFolderDoesNotExist);
			}

			folder.Index = await _folderRepository.CountChildFolders(parentFolderRef.Value);
			folder.Columns = parentFolder.Columns;
			folder.Rows = parentFolder.Rows;
		}

		await _folderRepository.Create(folder);

		UpdateTree(folder, parentFolder);

		await _folderRepository.Save();

		if (string.IsNullOrWhiteSpace(folder.Tree))
		{
			throw new InvalidOperationException("Tree is empty.");
		}

		await _publisherService.PublishNotification(new FolderCreatedNotification(folder));
		_logger.Information("Created folder {Folder} with tree {Tree}", folder.DisplayName, folder.Tree);

		return folder;
	}

	public async Task<FolderEntity> GetFolderById(int id)
	{
		var folder = await _folderRepository.GetFolderById(id);
		if (folder is null)
		{
			throw new ErrorCodeException(ErrorCode.NotFound);
		}

		return folder;
	}

	public async Task UpdateFolder(int id, FolderEntity folderUpdate)
	{
		var originalFolder = await _folderRepository.GetFolderById(id);
		if (originalFolder is null)
		{
			throw new ErrorCodeException(ErrorCode.NotFound);
		}

		if (!folderUpdate.DisplayName.Equals(originalFolder.DisplayName))
		{
			await ValidateDisplayName(folderUpdate.DisplayName, folderUpdate.ParentFolderRef);
			originalFolder.DisplayName = folderUpdate.DisplayName;
		}

		originalFolder.Index = folderUpdate.Index;
		originalFolder.Rows = folderUpdate.Rows;
		originalFolder.Columns = folderUpdate.Columns;

		if (originalFolder.Rows < 1)
		{
			originalFolder.Rows = 1;
		}

		if (originalFolder.Columns < 1)
		{
			originalFolder.Columns = 1;
		}

		if (originalFolder.Rows > 10)
		{
			originalFolder.Rows = 10;
		}

		if (originalFolder.Columns > 10)
		{
			originalFolder.Columns = 10;
		}

		if (folderUpdate.ParentFolderRef != originalFolder.ParentFolderRef)
		{
			await ValidateDisplayName(folderUpdate.DisplayName, folderUpdate.ParentFolderRef);
			originalFolder.ParentFolderRef = folderUpdate.ParentFolderRef;
		}

		await _folderRepository.Save();
		await _publisherService.PublishNotification(new FolderUpdatedNotification(originalFolder));
	}

	public async Task DeleteFolder(int id)
	{
		var folder = await _folderRepository.GetFolderById(id);
		if (folder is null)
		{
			throw new ErrorCodeException(ErrorCode.NotFound);
		}

		await _folderRepository.DeleteFolderByTree(folder.Tree);
		await _publisherService.PublishNotification(new FolderDeletedNotification(folder));
	}

	public Task<List<FolderEntity>> GetFolders()
	{
		return _folderRepository.GetFolders();
	}

	public async Task EnsureDefaultFolderExists()
	{
		var count = await _folderRepository.CountAllFolders();
		if (count > 0)
		{
			return;
		}

		await CreateFolder("Default Folder");
	}

	private static void UpdateTree(FolderEntity folder, FolderEntity? parentFolder = null)
	{
		folder.Tree = parentFolder is null ? $"{folder.Id:X}" : $"{parentFolder.Tree}-{folder.Id:X}";
	}

	private async Task ValidateDisplayName(string displayName, int? parentFolderId)
	{
		if (string.IsNullOrWhiteSpace(displayName))
		{
			throw new ErrorCodeException(ErrorCode.InvalidDisplayName);
		}

		if (await _folderRepository.DisplayNameExists(displayName, parentFolderId))
		{
			throw new ErrorCodeException(ErrorCode.FolderAlreadyExists);
		}
	}
}
