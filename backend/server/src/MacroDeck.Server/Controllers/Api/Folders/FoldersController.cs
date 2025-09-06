using MacroDeck.Server.Application.Extensions;
using MacroDeck.Server.Application.Services;
using MacroDeck.Server.Mappings;
using MacroDeck.Server.Models.Folders;
using Microsoft.AspNetCore.Mvc;

namespace MacroDeck.Server.Controllers.Api.Folders;

[Route("api/folders")]
public class FoldersController : BaseController
{
	private readonly FolderMapper _folderMapper;
	private readonly IFolderService _folderService;

	public FoldersController(
		IFolderService folderService,
		FolderMapper folderMapper)
	{
		_folderService = folderService;
		_folderMapper = folderMapper;
	}

	[HttpPost]
	public async Task<IActionResult> CreateFolder([FromBody] CreateFolderModel createFolder)
	{
		var folder = await _folderService.CreateFolder(createFolder.DisplayName,
													   createFolder.ParentFolderId);
		return Created(new Uri($"api/folders/{folder.Id}", UriKind.Relative),
					   _folderMapper.FolderEntityToFolderModel(folder));
	}

	[HttpGet]
	public async Task<IEnumerable<FolderWithChildsModel>> GetFolders()
	{
		var folders = await _folderService.GetFolders();
		var foldersTree = folders.ToTree(x => x.Id, x => x.ParentFolderRef);
		return _folderMapper.FolderTreeItemListToFolderWithChildsModelList(foldersTree);
	}

	[HttpGet("{id:int}")]
	public async Task<FolderModel> GetFolderBy(int id)
	{
		var folder = await _folderService.GetFolderById(id);
		return _folderMapper.FolderEntityToFolderModel(folder);
	}

	[HttpPut("{id:int}")]
	public async Task<IActionResult> UpdateFolder(int id, [FromBody] FolderModel folder)
	{
		await _folderService.UpdateFolder(id, _folderMapper.FolderModelToFolderEntity(folder));
		return Ok();
	}

	[HttpDelete("{id:int}")]
	public async Task<IActionResult> DeleteFolder(int id)
	{
		await _folderService.DeleteFolder(id);
		return Ok();
	}
}
