export class Field {
  name: string;
  value: string;
  constructor(name: string, value: string) {
    this.name = name;
    this.value = value;
  }
}
export const constants = {
  storage: {
    keys: {
      auth: 'iSHARE.auth',
      profile: 'iSHARE.profile',
      logging: 'iSHARE.logging',
      authExpiration: 'iSHARE.authExpiration',
      settings: 'iSHARE.settings'
    }
  },
  roles: {
    SchemeOwner: 'SchemeOwner',
    EntitledPartyViewer: 'EntitledPartyViewer',
    EntitledPartyCreator: 'EntitledPartyCreator'
  }
};
