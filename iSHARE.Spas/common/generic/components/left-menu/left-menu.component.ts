import { Component, OnInit, HostBinding, Input } from '@angular/core';
import { MenuItem } from '@common/generic/models/MenuItem';
import { AuthService } from '@common/generic/services/auth.service';
import { ProfileService } from '@common/generic/services/profile.service';

@Component({
  selector: 'app-left-menu',
  templateUrl: './left-menu.component.html',
  styleUrls: ['./left-menu.component.scss']
})
export class LeftMenuComponent implements OnInit {
  @Input()
  visible: boolean;
  @Input()
  menuItems: MenuItem[];
  @Input()
  activeMenuItems: MenuItem[];
  @HostBinding('class.active')
  active = false;

  hasVisibleEntries = false;
  constructor(private authService: AuthService, private profileService: ProfileService) {}

  ngOnInit() {
    this.applyPermissions();
    this.profileService.currentProfile.subscribe(profile => {
      this.applyPermissions();
    });
  }

  private applyPermissions() {
    this.menuItems.forEach(menuItem => {
      menuItem.visible = this.authService.inRole(menuItem.roles);
      if (menuItem.visible) {
        this.hasVisibleEntries = true;
      }
    });
    this.activeMenuItems = this.menuItems.filter(menuItem => menuItem.visible);
  }

  toggleActive() {
    this.active = !this.active;
  }
}
