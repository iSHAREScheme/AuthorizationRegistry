import { Injectable, Inject } from '@angular/core';
import { Profile } from '../models/Profile';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { Observable } from 'rxjs/internal/Observable';
import { constants } from '../../constants';
import { StorageKeysModel } from '../models/StorageKeysModel';
import { StorageService } from './storage.service';

@Injectable()
export class ProfileService {
  private profileBehaviourSubject: BehaviorSubject<Profile>;
  currentProfile: Observable<Profile>;

  constructor(private storageKeys: StorageKeysModel, private storageService: StorageService) {
    this.profileBehaviourSubject = new BehaviorSubject<Profile>(this.get());
    this.currentProfile = this.profileBehaviourSubject.asObservable();
  }

  set(profile: Profile): void {
    this.storageService.setItem(this.storageKeys.profile, JSON.stringify(profile));
    this.profileBehaviourSubject.next(profile);
  }

  get(): Profile {
    const rawProfileData = this.storageService.getItem(this.storageKeys.profile) || '{}';
    return JSON.parse(rawProfileData);
  }

  clear(): void {
    this.storageService.removeItem(this.storageKeys.profile);
    this.profileBehaviourSubject.next(undefined);
  }

  getRole(role: string) {
    switch (role) {
      case constants.roles.ArEntitledPartyCreator:
        return constants.rolesNames.ArEntitledPartyCreator;
      case constants.roles.ArEntitledPartyViewer:
        return constants.rolesNames.ArEntitledPartyViewer;
      case constants.roles.ArPartyAdmin:
        return constants.rolesNames.ArPartyAdmin;
      case constants.roles.SchemeOwner:
        return constants.rolesNames.SchemeOwner;
    }
  }
}
