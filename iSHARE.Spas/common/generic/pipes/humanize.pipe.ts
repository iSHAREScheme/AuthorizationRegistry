import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'humanize'
})
export class HumanizePipe implements PipeTransform {
  transform(value: any, args?: any): any {
    if (value.constructor === Array) {
      return value.map(v => this.huminizeCamelCaseString(v)).join(', ');
    }
    if (typeof value !== 'string') {
      return value;
    }
    return this.huminizeCamelCaseString(value);
  }

  private huminizeCamelCaseString(value: string): string {
    value = value.split(/(?=[A-Z])/).join(' ');
    value = value[0].toUpperCase() + value.slice(1);
    return value;
  }
}
