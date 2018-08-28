import { Directive, Input } from '@angular/core';
import { NG_VALIDATORS, Validator, AbstractControl } from '@angular/forms';
import { customControlRegexValidator } from './customControlRegexValidator';

@Directive({
  selector: '[appMinLowercase]',
  providers: [{ provide: NG_VALIDATORS, useExisting: MinLowercaseValidatorDirective, multi: true }]
})
export class MinLowercaseValidatorDirective implements Validator {
  // tslint:disable-next-line:no-input-rename
  @Input('appMinLowercase')
  minLowercase = 1;

  validate(control: AbstractControl): { [key: string]: any } | null {
    const pattern = new RegExp(`[a-z]{${this.minLowercase}}`);
    return this.minLowercase ? customControlRegexValidator(pattern, 'minLowercase')(control) : null;
  }
}
