import { TestBed, inject } from '@angular/core/testing';
import { PoliciesApiService } from './policies-api.service';

describe('Service: PoliciesApi', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [PoliciesApiService]
    });
  });

  it('should ...', inject([PoliciesApiService], (service: PoliciesApiService) => {
    expect(service).toBeTruthy();
  }));
});
