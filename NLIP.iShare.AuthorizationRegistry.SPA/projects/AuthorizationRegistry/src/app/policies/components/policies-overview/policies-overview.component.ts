import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { Router } from '@angular/router';
import { Pagination, constants, AlertService } from 'common';
import { OverviewPolicy } from '../../models/OverviewPolicy';
import { PoliciesApiService } from '../../services/policies-api.service';

@Component({
  selector: 'app-policies-overview',
  templateUrl: './policies-overview.component.html',
  styleUrls: ['./policies-overview.component.scss']
})
export class PoliciesOverviewComponent implements OnInit {
  policies: Observable<OverviewPolicy[]>;
  pagination: Pagination;
  roles = constants.roles;

  constructor(private api: PoliciesApiService, private router: Router, private alert: AlertService) {
    this.pagination = constants.paginationDefault;
  }

  ngOnInit() {
    this.loadData(this.pagination.page, this.pagination.pageSize);
  }

  loadData(page: number, pageSize: number) {
    this.api.getAll(page, pageSize).subscribe(response => {
      this.pagination.total = response.count;
      this.policies = of(response.data);
    });
  }

  onPageChange(page: number) {
    this.pagination.page = page;
    this.loadData(page, this.pagination.pageSize);
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
        this.loadData(this.pagination.page, this.pagination.pageSize);
      });
    }
  }

  download(policy: OverviewPolicy) {
    this.api.download(policy.id);
  }
}
