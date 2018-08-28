import { MaskApiService } from './mask-api.service';
import { TestBed, inject } from '@angular/core/testing';

describe('Service: PoliciesApi', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [MaskApiService]
    });
  });

  it('should ...', inject([MaskApiService], (service: MaskApiService) => {
    expect(service).toBeTruthy();
  }));
});
