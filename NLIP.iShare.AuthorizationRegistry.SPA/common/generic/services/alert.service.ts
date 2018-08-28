import { Injectable } from '@angular/core';
import * as alertifyjs from 'alertifyjs';

@Injectable()
export class AlertService {
  constructor() {
    alertifyjs.set('notifier', 'position', 'bottom-right'); // http://alertifyjs.com/notifier/position.html
  }

  error(message: string, delay: number = 0): any {
    return alertifyjs.error(message, delay);
  }

  success(message: string): any {
    return alertifyjs.success(message, 10);
  }
}
