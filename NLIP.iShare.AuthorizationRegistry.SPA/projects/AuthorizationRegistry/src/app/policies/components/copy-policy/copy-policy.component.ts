import { Component, OnInit, OnDestroy, ViewChild } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { PoliciesApiService } from '@app-ar/policies/services/policies-api.service';
import { JSONService, AlertService } from 'common';

@Component({
  selector: 'app-copy-policy',
  templateUrl: './copy-policy.component.html',
  styleUrls: ['./copy-policy.component.scss']
})
export class CopyPolicyComponent implements OnInit, OnDestroy {
  private paramsSubscription: any;
  @ViewChild('editor')
  editor;
  editorText: string;
  id: string;
  serverError: string;

  constructor(
    private route: ActivatedRoute,
    private api: PoliciesApiService,
    private router: Router,
    private JsonHelper: JSONService,
    private alert: AlertService
  ) {}

  ngOnInit() {
    this.paramsSubscription = this.route.params.subscribe(params => {
      this.id = params['id'];
      this.load(this.id);
    });
  }

  ngOnDestroy() {
    this.paramsSubscription.unsubscribe();
  }

  back() {
    this.router.navigate(['policies']);
  }

  save(): void {
    this.serverError = undefined;
    if (!this.JsonHelper.isValid(this.editorText)) {
      this.serverError = 'JSON format is incorrect.';
      return;
    }
    const policy = this.JsonHelper.trimJsonString(this.editorText);
    this.api.copy({ policy }).subscribe(
      () => {
        this.alert.success('Copy performed successfully');
        this.router.navigate(['policies']);
      },
      error => (this.serverError = error.message)
    );
  }

  private load(id: string): void {
    this.api.get(id).subscribe(response => {
      const parsed = JSON.parse(response.policy);
      this.editorText = JSON.stringify(parsed, null, '\t');
    });
  }
}
