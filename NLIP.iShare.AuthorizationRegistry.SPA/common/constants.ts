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
      auth: 'NLIP.iSHARE.auth',
      profile: 'NLIP.iSHARE.profile',
      logging: 'NLIP.iSHARE.logging',
      authExpiration: 'NLIP.iSHARE.authExpiration',
      settings: 'NLIP.iSHARE.settings'
    }
  },
  roles: {
    SchemeOwner: 'SchemeOwner',
    EntitledPartyViewer: 'EntitledPartyViewer',
    EntitledPartyCreator: 'EntitledPartyCreator'
  }
};
