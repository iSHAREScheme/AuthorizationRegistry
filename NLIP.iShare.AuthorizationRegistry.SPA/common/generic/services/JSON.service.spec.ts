import { TestBed, inject } from '@angular/core/testing';
import { JSONService } from './JSON.service';

describe('Service: JSON', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [JSONService]
    });
  });

  it('should ...', inject([JSONService], (service: JSONService) => {
    expect(service).toBeTruthy();
  }));
});
