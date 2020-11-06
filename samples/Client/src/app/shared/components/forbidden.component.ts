import { Component } from '@angular/core';

@Component({
  selector: 'tw-forbidden',
  template: `
    <h1>Oooppss... <br/>You do not have access to this area!</h1>
    <p>Please try to login or ask your administrator.</p>
  `
})
export class ForbiddenComponent {
}
