import { Observable, of } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProfileService, Pagination, Sorting, Profile, constants, AlertService } from 'common';
import { User } from '../../models/User';
import { UsersApiService } from '../../services/users-api.service';

@Component({
  selector: 'app-users-overview',
  templateUrl: './users-overview.component.html',
  styleUrls: ['./users-overview.component.scss']
})
export class UsersOverviewComponent implements OnInit {
  users: Observable<User[]>;
  pagination: Pagination;
  sorting: Sorting;
  currentUser: Profile;

  constructor(private api: UsersApiService, private router: Router, private alert: AlertService, private profile: ProfileService) {}

  ngOnInit() {
    this.currentUser = this.profile.get();
    this.initTableConnfiguration();
    this.loadData();
  }

  loadData() {
    this.api.getAll(this.pagination.page, this.pagination.pageSize, this.sorting.by, this.sorting.order).subscribe(response => {
      this.pagination.total = response.count;
      this.users = of(response.data);
    });
  }

  onPageChange(page: number) {
    this.pagination.page = page;
    this.loadData();
  }

  sort(by: string, order: 'asc' | 'desc') {
    this.sorting.by = by;
    this.sorting.order = order;
    this.loadData();
  }

  create() {
    this.router.navigate(['users', 'create']);
  }

  edit(user: User): void {
    this.router.navigate(['users', 'edit', user.id]);
  }

  delete(user: User) {
    if (confirm('Are you sure you want to delete this item?')) {
      this.api.delete(user.id).subscribe(r => {
        this.alert.success('Delete performed successfully');
        this.loadData();
      });
    }
  }

  initTableConnfiguration(): void {
    this.pagination = constants.paginationDefault;
    this.sorting = {
      by: 'partyId',
      order: 'asc'
    };
  }
}
