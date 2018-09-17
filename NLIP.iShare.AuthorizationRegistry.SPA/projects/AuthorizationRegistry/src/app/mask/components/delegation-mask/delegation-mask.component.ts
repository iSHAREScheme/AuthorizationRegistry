import { OnInit, Component } from '@angular/core';
import { Router } from '@angular/router';

import { DelegationResponse } from './../../models/DelegationResponse';
import { CreateDelegationMask } from './../../models/CreateDelegationMask';
import { AlertService } from 'common';
import { MaskApiService } from '@app-ar/mask/services/mask-api.service';

@Component({
  selector: 'app-delegation-mask',
  templateUrl: './delegation-mask.component.html',
  styleUrls: ['./delegation-mask.component.scss']
})
export class DelegationMaskComponent implements OnInit {
  delegationEvidence: DelegationResponse;
  delegationMask: CreateDelegationMask;
  serverError: string;
  showResponse = false;
  constructor(private api: MaskApiService, private alert: AlertService, private router: Router) {
    this.delegationMask = new CreateDelegationMask();
  }
  ngOnInit(): void {}
  back(): void {
    this.router.navigate(['policies']);
  }
  clear(): void {
    this.delegationMask = new CreateDelegationMask();
    this.delegationEvidence = null;
  }
  test(): void {
    this.api.test(this.delegationMask).subscribe(
      (data: any) => {
        this.alert.success('Success');
        this.delegationEvidence = data;
        this.showResponse = true;
        this.serverError = '';
      },
      error => {
        if (error.status === 404) {
          this.serverError = 'Combination policy issuer - access subject was not found.';
        } else {
          this.serverError = error.message;
        }
        this.delegationEvidence = null;
      }
    );
  }
  add(ids: Array<string>): void {
    ids.push('');
  }
  remove(ids: Array<string>, id: string): void {
    ids.splice(ids.indexOf(id), 1);
  }
  trackByFn(index: any, item: any): void {
    return index;
  }
}
