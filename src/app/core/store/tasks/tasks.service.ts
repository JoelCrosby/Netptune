import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AddProjectTaskRequest, ProjectTask } from '@core/models/project-task';
import { TaskViewModel } from '@core/models/view-models/project-task-dto';
import { environment } from '@env/environment';

@Injectable({
  providedIn: 'root',
})
export class ProjectTasksService {
  constructor(private http: HttpClient) {}

  get(workspaceSlug: string) {
    return this.http.get<TaskViewModel[]>(
      environment.apiEndpoint + `api/tasks?workspaceSlug=${workspaceSlug}`
    );
  }

  post(task: AddProjectTaskRequest) {
    return this.http.post<TaskViewModel>(
      environment.apiEndpoint + `api/tasks`,
      task
    );
  }

  put(task: ProjectTask) {
    return this.http.put<TaskViewModel>(
      environment.apiEndpoint + `api/tasks`,
      task
    );
  }

  delete(task: ProjectTask) {
    return this.http.delete<TaskViewModel>(
      environment.apiEndpoint + `api/tasks/${task.id}`
    );
  }

  detail(systemId: string, workspaceSlug: string) {
    return this.http.get<TaskViewModel>(
      environment.apiEndpoint + 'api/tasks/detail',
      {
        params: {
          workspace: workspaceSlug,
          systemId,
        },
      }
    );
  }
}