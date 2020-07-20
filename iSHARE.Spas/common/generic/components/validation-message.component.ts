import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-validation-message',
  template: '<div *ngIf="show" class="error-message"><ng-content></ng-content></div>'
})
export class ValidationMessageComponent {
  @Input()
  name: string;
  show = false;

  showsErrorIncludedIn(errors: string[]): boolean {
    return errors.some(error => error === this.name);
  }
}
