import { AuthorizedDirective } from './authorized.directive';

describe('Directive: Authorized', () => {
  it('should create an instance', () => {
    const directive = new AuthorizedDirective(null, null, null);
    expect(directive).toBeTruthy();
  });
});
