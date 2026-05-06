import { inject } from "@angular/core";
import { ActivatedRouteSnapshot, ResolveFn, RouterStateSnapshot } from "@angular/router";
import { Store } from "@ngxs/store";
import { InitializeNewWorkspace } from "../../store/workspace/workspace.actions";

export const newWorkspaceResolver: ResolveFn<unknown> = (
  route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot,
) => {
  return inject(Store).dispatch(new InitializeNewWorkspace());
}
