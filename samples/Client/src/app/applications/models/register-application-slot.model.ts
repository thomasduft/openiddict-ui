import {
  Slot,
  SINGLE_SLOT,
  Editor,
  TEXT_EDITOR,
  PASSWORD_EDITOR,
  CHECKBOX_EDITOR,
  SELECT_EDITOR,
  HIDDEN_EDITOR
} from '../../shared/formdef';

export class RegisterApplicationSlot implements Slot {
  public static KEY = 'RegisterApplicationSlot';

  public key = RegisterApplicationSlot.KEY;
  public type = SINGLE_SLOT;
  public title = 'Register';
  public editors: Editor[];

  public constructor() {

    this.editors = [
      {
        key: 'id',
        type: HIDDEN_EDITOR,
        label: 'Id',
        required: true
      },
      {
        key: 'clientId',
        type: TEXT_EDITOR,
        label: 'ClientId',
        required: true
      },
      {
        key: 'displayName',
        type: TEXT_EDITOR,
        label: 'Display name',
        required: true
      },
      {
        key: 'type',
        type: SELECT_EDITOR,
        required: true,
        label: 'Type',
        options: [
          { key: 'public', value: 'public' },
          { key: 'confidential', value: 'confidential' }
        ]
      },
      {
        key: 'clientSecret',
        type: PASSWORD_EDITOR,
        label: 'Client secret'
      },
      {
        key: 'requirePkce',
        type: CHECKBOX_EDITOR,
        label: 'Require Pkce',
        required: true
      },
      {
        key: 'requireConsent',
        type: CHECKBOX_EDITOR,
        label: 'Require consent'
      }
    ];
  }
}
