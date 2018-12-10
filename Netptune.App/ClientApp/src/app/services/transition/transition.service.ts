import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class TransitionService {

  public sidebarStateClass = 'open';

  constructor() { }

  toggleSideBar(): void {
    this.sidebarStateClass === 'open' ? this.sidebarStateClass = 'closed' : this.sidebarStateClass = 'open';
  }

}