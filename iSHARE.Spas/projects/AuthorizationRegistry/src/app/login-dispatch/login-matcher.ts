import { EnvironmentModel } from '@common/generic';
import { UrlSegment, UrlSegmentGroup, Route } from '@angular/router';
import { appInjector } from '../../main';

export function loginMatcher(segments: UrlSegment[], group: UrlSegmentGroup, route: Route) {

  const environement = appInjector.get(EnvironmentModel);
  const isPathMatch = segments[0].path === route.path;
  if (isPathMatch && environement.userManagement) {
    return { consumed: [segments[0]] };
  } else {
    return null;
  }
}
