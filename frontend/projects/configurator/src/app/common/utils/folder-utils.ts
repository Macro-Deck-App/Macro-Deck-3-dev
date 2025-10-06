import {FolderWithChildsModel} from '../models/folder-with-childs.model';

export function flattenFolders(folders: FolderWithChildsModel[]): FolderWithChildsModel[] {
  return folders.flatMap(folder => [
    {...folder, childs: []},
    ...flattenFolders(folder.childs)
  ]);
}
