import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from "@angular/router";
import { Store } from "@ngxs/store";
import { InitializeEditWorkspace, InitializeNewWorkspace } from "../../store/workspace/workspace.actions";

export const newWorkspaceResolver: ResolveFn<unknown> = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {
  return inject(Store).dispatch(new InitializeNewWorkspace());
}

export const editWorkspaceResolver: ResolveFn<unknown> = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {
  const id = route.paramMap.get('id');

  if (!id) {
    throw new Error('Missing workspace id');
  }

  return inject(Store).dispatch(new InitializeEditWorkspace(id));
}
