import { AbstractControl, ValidatorFn } from '@angular/forms';

export function customControlRegexValidator(nameRe: RegExp, errorName: string, value = 1): ValidatorFn {
  return (
    control: AbstractControl
  ): {
    [key: string]: any;
  } | null => {
    const isMatch = nameRe.test(control.value);
    const error = {};
    error[errorName] = {
      value: value
    };
    return isMatch ? null : error;
  };
}
