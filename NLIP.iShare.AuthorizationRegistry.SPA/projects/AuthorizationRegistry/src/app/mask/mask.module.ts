import { MaskRoutingModule } from './mask-routing.module';
import { MaskApiService } from './services/mask-api.service';
import { GenericModule as AppCommonModule } from 'common';
import { DelegationMaskComponent } from '@app-ar/mask/components/delegation-mask/delegation-mask.component';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';

@NgModule({
  imports: [CommonModule, AppCommonModule, MaskRoutingModule, FormsModule],
  declarations: [DelegationMaskComponent],
  providers: [MaskApiService]
})
export class MaskModule {}
