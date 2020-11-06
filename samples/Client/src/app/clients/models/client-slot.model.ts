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
    allowedGrantTypes: Array<string>,
    redirectUris: Array<string>,
    postLogoutRedirectUris: Array<string>,
    allowedCorsOrigins: Array<string>,
    allowedScopes: Array<string>
  ) {

    const allowedGrantTypesOptions = allowedGrantTypes.map((x: string) => {
      return { key: x, value: x };
    });

    const redirectUrisOptions = redirectUris.map((x: string) => {
      return { key: x, value: x };
    });

    const postLogoutRedirectUrisOptions = postLogoutRedirectUris.map((x: string) => {
      return { key: x, value: x };
    });

    const allowedCorsOriginsOptions = allowedCorsOrigins.map((x: string) => {
      return { key: x, value: x };
    });

    const allowedScopesOptions = allowedScopes.map((x: string) => {
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
        key: 'enabled',
        type: CHECKBOX_EDITOR,
        label: 'Enabled'
      },
      {
        key: 'clientId',
        type: TEXT_EDITOR,
        label: 'ClientId',
        required: true
      },
      {
        key: 'clientName',
        type: TEXT_EDITOR,
        label: 'Client name',
        required: true
      },
      {
        key: 'requireClientSecret',
        type: CHECKBOX_EDITOR,
        label: 'Require client secret',
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
        key: 'allowAccessTokensViaBrowser',
        type: CHECKBOX_EDITOR,
        label: 'Allow AccessTokens via Browser'
      },
      {
        key: 'allowedGrantTypes',
        type: MULTI_SELECT_EDITOR,
        label: 'Allowed GrantTypes',
        options: allowedGrantTypesOptions,
        singleSelection: false,
        bindingBehaviour: VALUE_BINDING_BEHAVIOR,
        allowAddingItems: true
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
      },
      {
        key: 'allowedCorsOrigins',
        type: MULTI_SELECT_EDITOR,
        label: 'Allowed CORS origins',
        options: allowedCorsOriginsOptions,
        singleSelection: false,
        bindingBehaviour: VALUE_BINDING_BEHAVIOR,
        allowAddingItems: true
      },
      {
        key: 'allowedScopes',
        type: MULTI_SELECT_EDITOR,
        label: 'Allowed scopes',
        options: allowedScopesOptions,
        singleSelection: false,
        bindingBehaviour: VALUE_BINDING_BEHAVIOR,
        allowAddingItems: false
      }
    ];
  }
}
