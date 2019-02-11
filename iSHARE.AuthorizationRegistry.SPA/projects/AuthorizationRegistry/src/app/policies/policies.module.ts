import { FormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NgxJsonViewerModule } from 'ngx-json-viewer';
import { AceEditorModule } from 'ng2-ace-editor';
import { NgxPaginationModule } from 'ngx-pagination';

import { PoliciesRoutingModule } from './policies-routing.module';
import { PoliciesApiService } from './services/policies-api.service';

import { GenericModule as AppCommonModule } from 'common';

import { PoliciesOverviewComponent } from './components/policies-overview/policies-overview.component';
import { ViewPolicyComponent } from './components/view-policy/view-policy.component';
import { CopyPolicyComponent } from './components/copy-policy/copy-policy.component';
import { EditPolicyComponent } from './components/edit-policy/edit-policy.component';
import { CreatePolicyComponent } from './components/create-policy/create-policy.component';
import { HistoryItemComponent } from './components/history-item/history-item.component';

@NgModule({
  imports: [FormsModule, CommonModule, AppCommonModule, PoliciesRoutingModule, NgxPaginationModule, NgxJsonViewerModule, AceEditorModule],
  declarations: [
    PoliciesOverviewComponent,
    ViewPolicyComponent,
    CopyPolicyComponent,
    EditPolicyComponent,
    CreatePolicyComponent,
    HistoryItemComponent
  ],
  providers: [PoliciesApiService]
})
export class PoliciesModule {}
