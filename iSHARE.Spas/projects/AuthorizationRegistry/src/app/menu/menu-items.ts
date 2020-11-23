import { MenuItem } from '@common/generic/models/MenuItem';

export const MENU_ITEMS: MenuItem[] = [
  {
    text: 'Policies',
    links: ['policies'],
    class: 'fa fa-file-text'
  },
  {
    text: 'Users',
    links: ['users'],
    class: 'fa fa-users'
  },
  {
    text: 'Test',
    links: ['test'],
    class: 'fa fa-play-circle'
  }
];
