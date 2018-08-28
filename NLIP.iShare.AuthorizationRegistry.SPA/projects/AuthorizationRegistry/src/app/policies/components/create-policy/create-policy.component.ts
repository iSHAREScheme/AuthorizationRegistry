import { Component, OnInit } from '@angular/core';
import { PoliciesApiService } from '@app-ar/policies/services/policies-api.service';
import { Router } from '@angular/router';
import { ProfileService, JSONService, AlertService } from 'common';

@Component({
  selector: 'app-create-policy',
  templateUrl: './create-policy.component.html',
  styleUrls: ['./create-policy.component.scss']
})
export class CreatePolicyComponent implements OnInit {
  editorText: string;
  serverError: string;
  constructor(
    private api: PoliciesApiService,
    private router: Router,
    private profile: ProfileService,
    private JsonHelper: JSONService,
    private alert: AlertService
  ) {}

  ngOnInit() {
    this.editorText = this.getBasicDelegationExample();
  }

  save(): void {
    this.serverError = undefined;
    if (!this.JsonHelper.isValid(this.editorText)) {
      this.serverError = 'JSON format is incorrect.';
      return;
    }
    const policy = this.JsonHelper.trimJsonString(this.editorText);
    this.api.create({ policy }).subscribe(
      () => {
        this.alert.success('Creation performed successfully');
        this.router.navigate(['policies']);
      },
      error => (this.serverError = error.data)
    );
  }

  back() {
    this.router.navigate(['policies']);
  }

  getBasicDelegationExample(): string {
    const partyId = this.profile.get().partyId || '';
    const date = Math.floor(new Date().getTime() / 1000);
    const delegation = {
      delegationEvidence: {
        notBefore: date,
        notOnOrAfter: date,
        policyIssuer: partyId,
        target: { accessSubject: '' },
        policySets: [
          {
            policies: [
              {
                target: {
                  resource: {
                    type: '',
                    identifiers: [''],
                    attributes: ['']
                  },
                  actions: ['ISHARE.CREATE', 'ISHARE.READ', 'ISHARE.UPDATE', 'ISHARE.DELETE']
                },
                rules: [
                  {
                    effect: 'Permit'
                  }
                ]
              }
            ]
          }
        ]
      }
    };
    return JSON.stringify(delegation, null, '\t');
  }
}
