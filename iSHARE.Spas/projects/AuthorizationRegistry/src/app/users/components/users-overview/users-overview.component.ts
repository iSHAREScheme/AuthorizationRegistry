import { Observable, of } from 'rxjs';
import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProfileService, Query, Profile, AlertService, EnvironmentModel, AuthService } from '@common/generic';
import { User } from '../../models/User';
import { UsersApiService } from '../../services/users-api.service';
import { constants } from '@common/constants';

@Component({
  selector: 'app-users-overview',
  templateUrl: './users-overview.component.html',
  styleUrls: ['./users-overview.component.scss']
})
export class UsersOverviewComponent implements OnInit {
  users: Observable<User[]>;
  query: Query;
  currentUser: Profile;
  hidePartyColumns = true;
  constructor(
    private api: UsersApiService,
    private router: Router,
    private alert: AlertService,
    private profile: ProfileService,
    private environment: EnvironmentModel,
    private auth: AuthService
  ) {
    this.environment = environment;
    this.hidePartyColumns = this.environment.disablePartyUsersManagement || !this.auth.inRole([constants.roles.SchemeOwner]);
  }

  ngOnInit() {
    this.currentUser = this.profile.get();
    this.query = new Query('username');
    this.loadData();
  }

  loadData() {
    this.api.getAll(this.query).subscribe(response => {
      this.query.total = response.count;
      this.users = of(response.data);
    });
  }

  onPageChange(page: number) {
    this.query.page = page;
    this.loadData();
  }

  search(filter: string): void {
    this.query.filter = filter;
    this.loadData();
  }

  sort($event: any) {
    this.query.sortBy = $event.by;
    this.query.sortOrder = $event.order;
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

  getRole(role: string) {
    return this.profile.getRole(role);
  }
}
