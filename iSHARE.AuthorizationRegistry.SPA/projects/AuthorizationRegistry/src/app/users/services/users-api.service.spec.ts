import { TestBed, inject } from '@angular/core/testing';
import { UsersApiService } from './users-api.service';

describe('Service: UsersApi', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [UsersApiService]
    });
  });

  it('should ...', inject([UsersApiService], (service: UsersApiService) => {
    expect(service).toBeTruthy();
  }));
});
