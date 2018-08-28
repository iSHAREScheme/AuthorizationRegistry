import { Injectable, Inject } from '@angular/core';
import { Profile } from '../models/Profile';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { Observable } from 'rxjs/internal/Observable';
import { EnvironmentModel } from '../models/EnvironmentModel';

@Injectable()
export class ProfileService {
  private profileBehaviourSubject: BehaviorSubject<Profile>;
  currentProfile: Observable<Profile>;
  environment: any;

  constructor(@Inject('environmentProvider') private environmentProvider: EnvironmentModel) {
    this.environment = environmentProvider;
    this.profileBehaviourSubject = new BehaviorSubject<Profile>(this.get());
    this.currentProfile = this.profileBehaviourSubject.asObservable();
  }

  set(profile: Profile): void {
    localStorage.setItem(this.environment.localStorageKeys.profile, JSON.stringify(profile));
    this.profileBehaviourSubject.next(profile);
  }

  get(): Profile {
    const rawProfileData = localStorage.getItem(this.environment.localStorageKeys.profile);
    return JSON.parse(rawProfileData);
  }

  clear(): void {
    localStorage.removeItem(this.environment.localStorageKeys.profile);
    this.profileBehaviourSubject.next(undefined);
  }
}
