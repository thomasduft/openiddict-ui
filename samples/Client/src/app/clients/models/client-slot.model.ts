import {
  Slot,
  SINGLE_SLOT,
  Editor,
  TEXT_EDITOR,
  CHECKBOX_EDITOR,
  MULTI_SELECT_EDITOR,
  VALUE_BINDING_BEHAVIOR,
  HIDDEN_EDITOR
} from '../../shared/formdef';

export class ClientDetailSlot implements Slot {
  public static KEY = 'ClientDetailSlot';

  public key = ClientDetailSlot.KEY;
  public type = SINGLE_SLOT;
  public title = 'Detail';
  public editors: Editor[];

  public constructor(
    redirectUris: Array<string>,
    postLogoutRedirectUris: Array<string>
  ) {

    const redirectUrisOptions = redirectUris.map((x: string) => {
      return { key: x, value: x };
    });

    const postLogoutRedirectUrisOptions = postLogoutRedirectUris.map((x: string) => {
      return { key: x, value: x };
    });

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
        key: 'clientSecret',
        type: TEXT_EDITOR,
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
      },
      {
        key: 'redirectUris',
        type: MULTI_SELECT_EDITOR,
        label: 'Redirect uris',
        options: redirectUrisOptions,
        singleSelection: false,
        bindingBehaviour: VALUE_BINDING_BEHAVIOR,
        allowAddingItems: true
      },
      {
        key: 'postLogoutRedirectUris',
        type: MULTI_SELECT_EDITOR,
        label: 'Post logout redirect uris',
        options: postLogoutRedirectUrisOptions,
        singleSelection: false,
        bindingBehaviour: VALUE_BINDING_BEHAVIOR,
        allowAddingItems: true
      }
    ];
  }
}
