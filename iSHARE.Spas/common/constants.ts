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
    ArEntitledPartyCreator: 'AR.EntitledPartyCreator',
    BcPartyAdmin: 'BC.PartyAdmin',
    BcEntitledPartyViewer: 'BC.EntitledPartyViewer',
    BcEntitledPartyCreator: 'BC.EntitledPartyCreator',
    CttPartyAdmin: 'CTT.PartyAdmin',
    CttPartyUser: 'CTT.PartyUser'
  },
  rolesNames: {
    SchemeOwner: 'Scheme Owner Administrator',
    ArPartyAdmin: 'AR Party Administrator',
    ArEntitledPartyViewer: 'AR Entitled Party Viewer',
    ArEntitledPartyCreator: 'AR Entitled Party Creator',
    BcPartyAdmin: 'BC Party Administrator',
    BcEntitledPartyViewer: 'BC Entitled Party Viewer',
    BcEntitledPartyCreator: 'BC Entitled Party Creator',
    CttPartyAdmin: 'CTT Party Administrator',
    CttPartyUser: 'CTT Party User'
  },
  categories: {
    AuthorizationRegistry: 'AuthorizationRegistry',
    BananaCo: 'BananaCo',
    ConformaneTestTool: 'ConformaneTestTool'
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
    },
    {
      category: 'Banana & Co',
      identifier: 'BananaCo',
      roles: [
        {
          value: 'BC.PartyAdmin',
          displayName: 'Party Admin'
        },
        {
          value: 'BC.EntitledPartyViewer',
          displayName: 'Entitled Party Viewer'
        },
        {
          value: 'BC.EntitledPartyCreator',
          displayName: 'Entitled Party Creator'
        }
      ]
    },
    {
      category: 'Conformance Test Tool',
      identifier: 'ConformaneTestTool',
      roles: [
        {
          value: 'CTT.PartyAdmin',
          displayName: 'Party Admin'
        },
        {
          value: 'CTT.PartyUser',
          displayName: 'Party User'
        }
      ]
    }
  ]
};
