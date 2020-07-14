import { TestBed, inject } from '@angular/core/testing';
import { AppInsightsService } from './app-insights.service';

describe('Service: Alert', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [AppInsightsService]
    });
  });

  it('should ...', inject([AppInsightsService], (service: AppInsightsService) => {
    expect(service).toBeTruthy();
  }));
});
