import {FolderModel} from './folder.model';

export interface FolderWithChildsModel extends FolderModel{
  childs: FolderWithChildsModel[];
}
