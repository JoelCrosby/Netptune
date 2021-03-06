import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '@env/environment';
import { BuildInfo } from './meta.model';

@Injectable({ providedIn: 'root' })
export class MetaService {
  constructor(private http: HttpClient) {}

  getBuildInfo() {
    return this.http.get<BuildInfo>(
      environment.apiEndpoint + 'api/meta/build-info'
    );
  }
}
