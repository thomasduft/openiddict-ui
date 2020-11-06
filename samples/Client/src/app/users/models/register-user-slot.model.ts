import {
  Slot,
  SINGLE_SLOT,
  Editor,
  TEXT_EDITOR,
  PASSWORD_EDITOR
} from '../../shared/formdef';

export class RegisterUserSlot implements Slot {
  public static KEY = 'RegisterUserSlot';

  public key = RegisterUserSlot.KEY;
  public type = SINGLE_SLOT;
  public title = 'Register user';
  public editors: Editor[];

  public constructor() {
    this.editors = [
      {
        key: 'userName',
        type: TEXT_EDITOR,
        label: 'Name',
        required: true
      },
      {
        key: 'email',
        type: TEXT_EDITOR,
        label: 'Email',
        required: true
      },
      {
        key: 'password',
        type: PASSWORD_EDITOR,
        label: 'Password',
        required: true,
        minLength: 6,
        maxLength: 100
      }
    ];
  }
}
