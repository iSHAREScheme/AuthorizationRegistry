import { PoliciesModule } from './policies.module';

describe('PoliciesModule', () => {
  let policiesModule: PoliciesModule;

  beforeEach(() => {
    policiesModule = new PoliciesModule();
  });

  it('should create an instance', () => {
    expect(policiesModule).toBeTruthy();
  });
});
