import { Directive, Input } from '@angular/core';
import { NG_VALIDATORS, Validator, AbstractControl } from '@angular/forms';
import { customControlRegexValidator } from './customControlRegexValidator';

@Directive({
  selector: '[appMinSpecialChars]',
  providers: [
    { provide: NG_VALIDATORS, useExisting: MinSpecialCharsValidatorDirective, multi: true }
  ]
})
export class MinSpecialCharsValidatorDirective implements Validator {
  // tslint:disable-next-line:no-input-rename
  @Input('appMinSpecialChars')
  minSpecialChars = 1;

  validate(control: AbstractControl): { [key: string]: any } | null {
    const pattern = new RegExp(`[^0-9a-zA-Z]{${this.minSpecialChars}}`, 'i');
    return this.minSpecialChars
      ? customControlRegexValidator(pattern, 'minSpecialChars')(control)
      : null;
  }
}
