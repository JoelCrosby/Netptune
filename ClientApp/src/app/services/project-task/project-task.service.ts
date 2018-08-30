import { Injectable, Inject } from '@angular/core';
import { HttpHeaders, HttpErrorResponse, HttpClient } from '@angular/common/http';
import { Workspace } from '../../models/workspace';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AuthService } from '../auth/auth.service';
import { ProjectTask } from '../../models/project-task';
import { WorkspaceService } from '../workspace/workspace.service';

@Injectable({
  providedIn: 'root'
})
export class ProjectTaskService {

  public tasks: ProjectTask[];

  constructor(
    private http: HttpClient,
    private authService: AuthService,
    private workspaceService: WorkspaceService,
    @Inject('BASE_URL') private baseUrl: string) { }

  refreshTasks(workspace): void {
    this.getTasks(workspace ? workspace : this.workspaceService.currentWorkspace)
      .subscribe(Tasks => this.tasks = Tasks);
  }

  getHeaders() {
    return {
      headers: new HttpHeaders({
        'Content-Type': 'application/json',
        'Authorization': 'Bearer ' + this.authService.token.token
      })
    };
  }

  getTasks(worspace: Workspace): Observable<ProjectTask[]> {
    const httpOptions = this.getHeaders();

    return this.http.get<ProjectTask[]>(this.baseUrl + 'api/ProjectTasks' + '?workspaceId=' + worspace.workspaceId, httpOptions)
      .pipe(
        catchError(this.handleError)
      );
  }

  addTask(task: ProjectTask): Observable<ProjectTask> {
    const httpOptions = this.getHeaders();

    if (!task.workspace) {
      task.workspace = this.workspaceService.currentWorkspace;
    }

    return this.http.post<ProjectTask>(this.baseUrl + 'api/ProjectTasks', task, httpOptions)
      .pipe(
        catchError(this.handleError)
      );
  }

  updateTask(task: ProjectTask): Observable<ProjectTask> {
    const httpOptions = this.getHeaders();

    const url = `${this.baseUrl}api/ProjectTasks/${task.taskId}`;
    return this.http.put<ProjectTask>(url, ProjectTask, httpOptions)
      .pipe(
        catchError(this.handleError)
      );
  }

  deleteTask(task: ProjectTask): Observable<ProjectTask> {
    const httpOptions = this.getHeaders();

    const url = `${this.baseUrl}api/ProjectTasks/${task.taskId}`;
    return this.http.delete<ProjectTask>(url, httpOptions)
      .pipe(
        catchError(this.handleError)
      );
  }

  private handleError(error: HttpErrorResponse) {
    if (error.error instanceof ErrorEvent) {
      // A client-side or network error occurred. Handle it accordingly.
      console.error('An error occurred:', error.error.message);
    } else {
      // The backend returned an unsuccessful response code.
      // The response body may contain clues as to what went wrong,
      console.error(
        `Backend returned code ${error.status}, ` +
        `body was: ${error.error}`);
    }
    // return an observable with a user-facing error message
    return throwError(
      'Something bad happened; please try again later.');
  }
}
