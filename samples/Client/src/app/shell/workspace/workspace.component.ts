import { Component, HostBinding } from '@angular/core';

@Component({
  selector: 'tw-workspace',
  templateUrl: './workspace.component.html'
})
export class WorkspaceComponent {
  @HostBinding('class')
  public style = 'workspace';
}
