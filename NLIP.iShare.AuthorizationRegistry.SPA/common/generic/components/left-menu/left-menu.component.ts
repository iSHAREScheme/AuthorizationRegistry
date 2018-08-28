import { Component, OnInit, HostBinding, Input } from '@angular/core';
import { constants } from '@common/constants';

@Component({
  selector: 'app-left-menu',
  templateUrl: './left-menu.component.html',
  styleUrls: ['./left-menu.component.scss']
})
export class LeftMenuComponent implements OnInit {
  @Input()
  visible: boolean;

  @HostBinding('class.active')
  active = false;

  roles = constants.roles;

  constructor() {}

  ngOnInit() {}
}
