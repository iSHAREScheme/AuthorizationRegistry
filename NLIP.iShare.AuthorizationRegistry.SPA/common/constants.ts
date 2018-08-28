export class Field {
  name: string;
  value: string;
  constructor(name: string, value: string) {
    this.name = name;
    this.value = value;
  }
}

export const constants = {
  roles: {
    SchemeOwner: 'SchemeOwner',
    EntitledPartyViewer: 'EntitledPartyViewer',
    EntitledPartyCreator: 'EntitledPartyCreator'
  },
  paginationDefault: {
    pageSize: 10,
    page: 1,
    total: 0
  }
};
