import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Router } from '@angular/router';
import { Query, constants, AlertService } from 'common';
import { OverviewPolicy } from '../../models/OverviewPolicy';
import { PoliciesApiService } from '../../services/policies-api.service';

@Component({
  selector: 'app-policies-overview',
  templateUrl: './policies-overview.component.html',
  styleUrls: ['./policies-overview.component.scss']
})
export class PoliciesOverviewComponent implements OnInit {
  policies: Observable<OverviewPolicy[]>;
  query: Query;
  roles = constants.roles;

  constructor(private api: PoliciesApiService, private router: Router, private alert: AlertService) {}

  ngOnInit() {
    this.query = new Query('arIdentifier');
    this.loadData();
  }

  loadData() {
    this.api.getAll(this.query).subscribe(response => {
      this.query.total = response.count;
      this.policies = of(response.data);
    });
  }

  search(filter: string): void {
    this.query.filter = filter;
    this.loadData();
  }

  sort($event) {
    this.query.sortBy = $event.by;
    this.query.sortOrder = $event.order;
    this.loadData();
  }

  onPageChange(page: number) {
    this.query.page = page;
    this.loadData();
  }

  view(policy: OverviewPolicy) {
    this.router.navigate(['policies', 'view', policy.authorizationRegistryId]);
  }

  copy(policy: OverviewPolicy) {
    this.router.navigate(['policies', 'copy', policy.authorizationRegistryId]);
  }

  create() {
    this.router.navigate(['policies', 'create']);
  }

  edit(policy: OverviewPolicy) {
    this.router.navigate(['policies', 'edit', policy.authorizationRegistryId]);
  }

  delete(policy: OverviewPolicy) {
    if (confirm('Are you sure you want to delete this item?')) {
      this.api.delete(policy.authorizationRegistryId).subscribe(r => {
        this.alert.success('Delete performed successfully');
        this.loadData();
      });
    }
  }

  download(policy: OverviewPolicy) {
    this.api.download(policy.id);
  }
}
