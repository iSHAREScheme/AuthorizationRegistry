export interface User {
  id: string;
  partyId: string;
  partyName: string;
  email: string;
  username: string;
  roles: string[];
  createdDate: Date;
  active: string;
}
