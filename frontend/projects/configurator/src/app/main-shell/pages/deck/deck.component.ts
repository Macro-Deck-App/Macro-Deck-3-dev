import { Component } from '@angular/core';
import {FolderNavigationComponent} from './folder-navigation/folder-navigation.component';
import {WidgetsContainerComponent} from './widgets-container/widgets-container.component';

@Component({
  selector: 'app-deck',
  imports: [
    FolderNavigationComponent,
    WidgetsContainerComponent
  ],
  templateUrl: './deck.component.html'
})
export class DeckComponent {

}
