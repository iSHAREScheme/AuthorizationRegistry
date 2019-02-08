import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxPaginationModule } from 'ngx-pagination';
import { GenericModule } from 'common';
import { UsersRoutingModule } from './users-routing.module';
import { UsersOverviewComponent } from './components/users-overview/users-overview.component';
import { UsersApiService } from './services/users-api.service';
import { CreateUserComponent } from './components/create-user/create-user.component';
import { EditUserComponent } from './components/edit-user/edit-user.component';

@NgModule({
  imports: [CommonModule, GenericModule, UsersRoutingModule, NgxPaginationModule, FormsModule, ReactiveFormsModule],
  declarations: [UsersOverviewComponent, CreateUserComponent, EditUserComponent],
  providers: [UsersApiService]
})
export class UsersModule {}
