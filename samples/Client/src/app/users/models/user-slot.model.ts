import {
  Slot,
  SINGLE_SLOT,
  Editor,
  HIDDEN_EDITOR,
  TEXT_EDITOR,
  CHECKBOX_EDITOR,
  MULTI_SELECT_EDITOR,
  VALUE_BINDING_BEHAVIOR,
  ARRAY_SLOT,
  SELECT_EDITOR
} from '../../shared/formdef';

export class UserDetailSlot implements Slot {
  public static KEY = 'UserDetailSlot';

  public key = UserDetailSlot.KEY;
  public type = SINGLE_SLOT;
  public title = 'Detail';
  public editors: Editor[];
  public children: Array<Slot>;

  public constructor(
    claims: Array<string>,
    roles: Array<string>
  ) {
    const claimOptions = claims.map((c: string) => {
      return { key: c, value: c };
    });

    const roleOptions = roles.map((r: string) => {
      return { key: r, value: r };
    });

    this.editors = [
      {
        key: 'id',
        type: HIDDEN_EDITOR,
        label: 'Id',
        required: true
      },
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
        key: 'lockoutEnabled',
        type: CHECKBOX_EDITOR,
        label: 'Lockout enabled'
      },
      {
        key: 'isLockedOut',
        type: CHECKBOX_EDITOR,
        label: 'Is locked out',
        isReadOnly: true
      },
      {
        key: 'roles',
        type: MULTI_SELECT_EDITOR,
        label: 'Roles',
        required: false,
        options: roleOptions,
        singleSelection: false,
        bindingBehaviour: VALUE_BINDING_BEHAVIOR
      }
    ];

    this.children = [
      {
        key: 'claims',
        type: ARRAY_SLOT,
        title: 'Claims',
        editors: [
          {
            key: 'type',
            type: SELECT_EDITOR,
            label: 'Claims',
            required: true,
            options: claimOptions,
            singleSelection: true,
            bindingBehaviour: VALUE_BINDING_BEHAVIOR
          },
          {
            key: 'value',
            type: TEXT_EDITOR,
            label: 'Value',
            required: true
          }
        ]
      }
    ];
  }
}
