import { Component, OnInit, Input, ContentChildren, QueryList, OnDestroy } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ValidationMessageComponent } from './validation-message.component';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-display-errors',
  template: '<ng-content></ng-content>'
})
export class DisplayErrorsComponent implements OnInit, OnDestroy {
  @Input()
  for: FormControl;
  @ContentChildren(ValidationMessageComponent)
  messages: QueryList<ValidationMessageComponent>;

  private statusChangesSubscription: Subscription;

  ngOnInit() {
    this.statusChangesSubscription = this.for.statusChanges.subscribe(x => {
      this.messages.forEach(messageComponent => (messageComponent.show = false));

      if (this.for.invalid) {
        const first = this.messages.find(message => {
          return message.showsErrorIncludedIn(Object.keys(this.for.errors));
        });
        if (!!first) {
          first.show = true;
        }
      }
    });
  }

  ngOnDestroy() {
    this.statusChangesSubscription.unsubscribe();
  }
}
