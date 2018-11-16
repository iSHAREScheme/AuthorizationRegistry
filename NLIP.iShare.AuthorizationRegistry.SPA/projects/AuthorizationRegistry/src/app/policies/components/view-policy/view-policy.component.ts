import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import * as _ from 'lodash';
import { PoliciesApiService } from '../../services/policies-api.service';
import { Policy } from '../../models/Policy';

@Component({
  selector: 'app-view-policy',
  templateUrl: './view-policy.component.html',
  styleUrls: ['./view-policy.component.scss']
})
export class ViewPolicyComponent implements OnInit, OnDestroy {
  private paramsSubscription: any;
  model: Policy;
  id: string;
  parsedForViewer: any;
  historyVisible = false;
  loading = true;

  constructor(private route: ActivatedRoute, private api: PoliciesApiService, private router: Router) {}

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

  private load(id: string): void {
    this.api.get(id).subscribe(
      response => {
        this.model = response;
        this.model.history = _.sortBy(this.model.history, item => new Date(item.createdDate)).reverse();
        this.parsedForViewer = JSON.parse(response.policy);
        this.loading = false;
      },
      err => {
        this.loading = false;
        if (err.status === 404) {
          this.router.navigate(['not-found'], { skipLocationChange: true });
        }
      }
    );
  }
}
