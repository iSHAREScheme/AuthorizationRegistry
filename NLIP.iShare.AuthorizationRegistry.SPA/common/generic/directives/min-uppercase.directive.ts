import { Directive, Input } from '@angular/core';
import { NG_VALIDATORS, Validator, AbstractControl } from '@angular/forms';

import { customControlRegexValidator } from './customControlRegexValidator';

@Directive({
  selector: '[appMinUppercase]',
  providers: [{ provide: NG_VALIDATORS, useExisting: MinUppercaseValidatorDirective, multi: true }]
})
export class MinUppercaseValidatorDirective implements Validator {
  // tslint:disable-next-line:no-input-rename
  @Input('appMinUppercase')
  minUppercase = 1;

  validate(control: AbstractControl): { [key: string]: any } | null {
    const pattern = new RegExp(`[A-Z]{${this.minUppercase}}`);
    return this.minUppercase ? customControlRegexValidator(pattern, 'minUppercase')(control) : null;
  }
}
