import { Injectable, Inject } from '@angular/core';
import { Profile } from '../models/Profile';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { Observable } from 'rxjs/internal/Observable';
import { constants } from '../../constants';

@Injectable()
export class ProfileService {
  private profileBehaviourSubject: BehaviorSubject<Profile>;
  currentProfile: Observable<Profile>;

  constructor() {
    this.profileBehaviourSubject = new BehaviorSubject<Profile>(this.get());
    this.currentProfile = this.profileBehaviourSubject.asObservable();
  }

  set(profile: Profile): void {
    localStorage.setItem(constants.storage.keys.profile, JSON.stringify(profile));
    this.profileBehaviourSubject.next(profile);
  }

  get(): Profile {
    const rawProfileData = localStorage.getItem(constants.storage.keys.profile);
    return JSON.parse(rawProfileData);
  }

  clear(): void {
    localStorage.removeItem(constants.storage.keys.profile);
    this.profileBehaviourSubject.next(undefined);
  }

  getRole(role: string) {
    switch (role) {
        case constants.roles.ArEntitledPartyCreator: return constants.rolesNames.ArEntitledPartyCreator;
        case constants.roles.ArEntitledPartyViewer: return constants.rolesNames.ArEntitledPartyViewer;
        case constants.roles.ArPartyAdmin: return constants.rolesNames.ArPartyAdmin;
        case constants.roles.SchemeOwner: return constants.rolesNames.SchemeOwner;
    }
  }
}
