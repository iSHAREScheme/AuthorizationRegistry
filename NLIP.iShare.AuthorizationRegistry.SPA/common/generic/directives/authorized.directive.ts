import { Directive, Input, TemplateRef, ViewContainerRef, OnInit } from '@angular/core';
import { AuthService } from '../services/auth.service';

@Directive({
  selector: '[appAllowedForRoles]'
})
export class AuthorizedDirective implements OnInit {
  @Input()
  appAllowedForRoles: string[];
  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef,
    private auth: AuthService
  ) {}

  ngOnInit(): void {
    this.applyPermission();
  }

  applyPermission() {
    this.auth.authorize(this.appAllowedForRoles).subscribe(authorized => {
      if (authorized) {
        this.viewContainer.createEmbeddedView(this.templateRef);
      } else {
        this.viewContainer.clear();
      }
    });
  }
}
