import {
  Slot,
  SINGLE_SLOT,
  Editor,
  TEXT_EDITOR,
  CHECKBOX_EDITOR,
  MULTI_SELECT_EDITOR,
  VALUE_BINDING_BEHAVIOR,
  HIDDEN_EDITOR,
  TEXT_AREA_EDITOR
} from '../../shared/formdef';

export class ScopeDetailSlot implements Slot {
  public static KEY = 'ScopeDetailSlot';

  public key = ScopeDetailSlot.KEY;
  public type = SINGLE_SLOT;
  public title = 'Detail';
  public editors: Editor[];

  public constructor(claims: Array<string>) {
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
        key: 'description',
        type: TEXT_AREA_EDITOR,
        label: 'Description'
      },
      {
        key: 'required',
        type: CHECKBOX_EDITOR,
        label: 'Required'
      },
      {
        key: 'emphasize',
        type: CHECKBOX_EDITOR,
        label: 'Emphasize'
      },
      {
        key: 'showInDiscoveryDocument',
        type: CHECKBOX_EDITOR,
        label: 'Show in discovery document'
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
