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
    ArPartyAdmin: 'AR.PartyAdmin',
    ArEntitledPartyViewer: 'AR.EntitledPartyViewer',
    ArEntitledPartyCreator: 'AR.EntitledPartyCreator'
  },
  rolesNames: {
    SchemeOwner: 'Scheme Owner Administrator',
    ArPartyAdmin: 'AR Party Administrator',
    ArEntitledPartyViewer: 'AR Entitled Party Viewer',
    ArEntitledPartyCreator: 'AR Entitled Party Creator'
  },
  categories: {
    AuthorizationRegistry: 'AuthorizationRegistry'
  },
  roleCategories: [
    {
      category: 'Authorization Registry',
      identifier: 'AuthorizationRegistry',
      roles: [
        {
          value: 'AR.PartyAdmin',
          displayName: 'Party Admin'
        },
        {
          value: 'AR.EntitledPartyViewer',
          displayName: 'Entitled Party Viewer'
        },
        {
          value: 'AR.EntitledPartyCreator',
          displayName: 'Entitled Party Creator'
        }
      ]
    }
  ]
};
