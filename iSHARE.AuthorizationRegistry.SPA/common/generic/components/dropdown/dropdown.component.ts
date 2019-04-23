import { Component, Input, forwardRef, HostListener, ElementRef } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { Field } from '@common/constants';

@Component({
  selector: 'app-dropdown',
  templateUrl: './dropdown.component.html',
  styleUrls: ['./dropdown.component.scss'],
  providers: [
    { provide: NG_VALUE_ACCESSOR, useExisting: forwardRef(() => DropdownComponent), multi: true }
  ]
})
export class DropdownComponent implements ControlValueAccessor {
  @Input()
  items: any[] = [];
  @Input()
  title: string;
  @Input()
  disable: boolean;

  selectedValue: object;
  dropdownOpen = false;

  onChange;
  onTouch;

  viewValue = item => (item instanceof Field ? item.name : item);
  modelValue = item => (item instanceof Field ? item.value : item);

  constructor(private elementRef: ElementRef) {}

  @HostListener('document:click', ['$event'])
  clickOutside(event) {
    if (!this.elementRef.nativeElement.contains(event.target)) {
      this.dropdownOpen = false;
    }
  }

  selectItem(index) {
    this.selectedValue = this.items[index];
    this.onChange(this.modelValue(this.selectedValue));
  }

  writeValue(value) {
    if (!this.items) {
      return;
    }
    let item: object;
    if (value instanceof Field) {
      item = this.items.find(
        a => (a instanceof Field ? a.value === value.value : a === value.value)
      );
    } else {
      item = this.items.find(a => (a instanceof Field ? a.value === value : a === value));
    }
    this.selectedValue = item;
  }
  registerOnChange(fn) {
    this.onChange = fn;
  }

  registerOnTouched(fn) {
    this.onTouch = fn;
  }
}
