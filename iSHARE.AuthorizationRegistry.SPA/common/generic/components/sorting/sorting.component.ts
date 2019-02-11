import { Component, Input, OnInit, Output, EventEmitter } from '@angular/core';

@Component({
  selector: 'app-sorting',
  templateUrl: './sorting.component.html',
  styleUrls: ['./sorting.component.scss']
})
export class SortingComponent implements OnInit {
  @Input()
  by: string;

  @Input()
  order: 'asc' | 'desc';

  @Output()
  action = new EventEmitter<any>();

  ngOnInit(): void {}

  setSorting(): void {
    this.order = this.order === 'asc' ? 'desc' : 'asc';
    this.action.emit({ order: this.order, by: this.by });
  }
}
