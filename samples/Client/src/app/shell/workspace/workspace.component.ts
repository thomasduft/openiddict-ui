import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'tw-workspace',
  templateUrl: './workspace.component.html',
  styleUrls: ['./workspace.component.less']
})
export class WorkspaceComponent {
  @HostBinding('class')
  public style = 'workspace';
}
