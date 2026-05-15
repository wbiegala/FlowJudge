import { DataGridRow } from "@flow-judge-webapp/ui";
import { EnumValue } from "@flow-judge-webapp/common";
import { WorkspaceRole } from "./workspace-shared.model";

export interface WorkspaceGridItem extends DataGridRow {
  name: string;
  ownerEmail: string;
  role: EnumValue<WorkspaceRole>;
  createdAt: Date;
}


