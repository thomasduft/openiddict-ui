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

export class ResourceDetailSlot implements Slot {
  public static KEY = 'ResourceDetailSlot';

  public key = ResourceDetailSlot.KEY;
  public type = SINGLE_SLOT;
  public title = 'Detail';
  public editors: Editor[];

  public constructor(scopes: Array<string>, claims: Array<string>) {

    const scopeOptions = scopes.map((s: string) => {
      return { key: s, value: s };
    });

    const claimOptions = claims.map((c: string) => {
      return { key: c, value: c };
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
        key: 'name',
        type: TEXT_EDITOR,
        label: 'Name',
        required: true
      },
      {
        key: 'displayName',
        type: TEXT_EDITOR,
        label: 'Display name',
        required: true
      },
      {
        key: 'scopes',
        type: MULTI_SELECT_EDITOR,
        label: 'Scopes',
        options: scopeOptions,
        singleSelection: false,
        bindingBehaviour: VALUE_BINDING_BEHAVIOR,
        allowAddingItems: false
      }
      // {
      //   key: 'userClaims',
      //   type: MULTI_SELECT_EDITOR,
      //   label: 'User claims',
      //   options: claimOptions,
      //   singleSelection: false,
      //   bindingBehaviour: VALUE_BINDING_BEHAVIOR,
      //   allowAddingItems: true
      // }
    ];
  }
}
