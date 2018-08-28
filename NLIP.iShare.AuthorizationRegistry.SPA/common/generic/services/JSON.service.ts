import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class JSONService {
  constructor() {}

  isValid(jsonAsString): boolean {
    try {
      JSON.parse(jsonAsString);
      return true;
    } catch (error) {
      return false;
    }
  }

  trimJsonString(jsonAsString): string {
    try {
      return JSON.stringify(JSON.parse(jsonAsString), null, 0);
    } catch (e) {
      return null;
    }
  }
}
