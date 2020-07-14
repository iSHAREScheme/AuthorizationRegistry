import { Component, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-search',
  templateUrl: './search.component.html',
  styleUrls: ['./search.component.scss']
})
export class SearchComponent {
  @Output()
  action = new EventEmitter<string>();

  filter: string;

  setSearch(): void {
    this.action.emit(this.filter);
  }
}
