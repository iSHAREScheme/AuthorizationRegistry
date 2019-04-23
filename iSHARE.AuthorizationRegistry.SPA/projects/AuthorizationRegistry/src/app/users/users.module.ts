import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgxPaginationModule } from 'ngx-pagination';
import { UsersRoutingModule } from './users-routing.module';
import { UsersOverviewComponent } from './components/users-overview/users-overview.component';
import { UsersApiService } from './services/users-api.service';
import { CreateUserComponent } from './components/create-user/create-user.component';
import { EditUserComponent } from './components/edit-user/edit-user.component';
import { GenericModule } from '@common/generic';
import { NgMultiSelectDropDownModule } from 'ng-multiselect-dropdown';

@NgModule({
  imports: [
    CommonModule,
    GenericModule,
    UsersRoutingModule,
    NgxPaginationModule,
    FormsModule,
    ReactiveFormsModule,
    NgMultiSelectDropDownModule
  ],
  declarations: [UsersOverviewComponent, CreateUserComponent, EditUserComponent],
  providers: [UsersApiService, ]
})
export class UsersModule {}
