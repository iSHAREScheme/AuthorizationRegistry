import { MenuItem } from '@common/generic/models/MenuItem';
import { constants } from '@common/constants';

export const MENU_ITEMS: MenuItem[] = [
  {
    text: 'Policies',
    links: ['policies'],
    roles: [constants.roles.SchemeOwner, constants.roles.EntitledPartyViewer, constants.roles.EntitledPartyCreator],
    class: 'fa fa-file-text'
  },
  {
    text: 'Users',
    links: ['users'],
    roles: [constants.roles.SchemeOwner],
    class: 'fa fa-users'
  },
  {
    text: 'Test',
    links: ['test'],
    roles: [constants.roles.SchemeOwner, constants.roles.EntitledPartyViewer, constants.roles.EntitledPartyCreator],
    class: 'fa fa-play-circle'
  }
];
