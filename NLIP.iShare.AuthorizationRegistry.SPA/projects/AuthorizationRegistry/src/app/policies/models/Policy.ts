export interface Policy {
  policyIssuer: string;
  accessSubject: string;
  authorizationRegistryId: string;
  policy: string;
  createdBy: string;
  createdDate: Date;
  history?: Policy[];
  id: string;
}
