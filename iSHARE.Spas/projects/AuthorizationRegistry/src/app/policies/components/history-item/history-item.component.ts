import { Component, OnInit, Input } from '@angular/core';
import { Policy } from '../../models/Policy';
import { PoliciesApiService } from '../../services/policies-api.service';

@Component({
  selector: 'app-history-item',
  templateUrl: './history-item.component.html',
  styleUrls: ['./history-item.component.scss']
})
export class HistoryItemComponent implements OnInit {
  @Input()
  item: Policy & { expanded: boolean; parsedJson: any };
  @Input()
  authorizationRegistryId: string;

  constructor(private api: PoliciesApiService) {}

  ngOnInit() {
    const json = JSON.parse(this.item.policy);
    this.item.parsedJson = json;
    this.item.policyIssuer = json.delegationEvidence.policyIssuer;
    this.item.accessSubject = json.delegationEvidence.target.accessSubject;
    this.item.authorizationRegistryId = this.authorizationRegistryId;
  }

  download(historyItem: Policy) {
    this.api.history.download(historyItem.id);
  }
}
