import { customControlRegexValidator } from './customControlRegexValidator';

import { Directive, Input } from '@angular/core';
import { NG_VALIDATORS, Validator, AbstractControl } from '@angular/forms';

@Directive({
  selector: '[appMinNumeric]',
  providers: [{ provide: NG_VALIDATORS, useExisting: MinNumericValidatorDirective, multi: true }]
})
export class MinNumericValidatorDirective implements Validator {
  // tslint:disable-next-line:no-input-rename
  @Input('appMinNumeric')
  minNumeric = 1;

  validate(control: AbstractControl): { [key: string]: any } | null {
    const pattern = new RegExp('[0-9]{' + this.minNumeric + '}');
    return this.minNumeric ? customControlRegexValidator(pattern, 'minNumeric')(control) : null;
  }
}
