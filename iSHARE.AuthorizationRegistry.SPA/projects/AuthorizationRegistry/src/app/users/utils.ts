import * as _ from 'lodash';
import { constants } from '@common/constants';

export const Utils = {
  addRole(items: any[], category: string) {
    items.push(_.find(constants.roleCategories, x => x.identifier === category));
  },
  createRoleCategories(items: any[]) {
    return [].concat.apply(
      [],
      _.map(items, y => {
        return [{ value: y.identifier, displayName: y.category, isHeader: true }].concat(
          _.map(y.roles, z => {
            return {
              value: z.value,
              displayName: z.displayName,
              isHeader: false
            };
          })
        );
      })
    );
  }
};
