import { AsyncEntityState } from '@core/util/entity/async-entity-state';
import { ProjectViewModel } from '@core/models/view-models/project-view-model';
import { createEntityAdapter } from '@ngrx/entity';

export const adapter = createEntityAdapter<ProjectViewModel>();

export const initialState: ProjectsState = adapter.getInitialState({
  loading: true,
  loaded: false,
  loadingCreate: false,
  currentProject: undefined,
  projectDetail: undefined,
  projectDetailLoading: true,
  projectUpdateLoading: false,
});

export interface ProjectsState extends AsyncEntityState<ProjectViewModel> {
  currentProject?: ProjectViewModel;
  projectDetail?: ProjectViewModel;
  projectDetailLoading: boolean;
  projectUpdateLoading: boolean;
}
