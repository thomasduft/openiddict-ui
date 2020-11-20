import {
  Slot,
  SINGLE_SLOT,
  Editor,
  TEXT_EDITOR,
  HIDDEN_EDITOR,
  TEXT_AREA_EDITOR,
  MULTI_SELECT_EDITOR,
  VALUE_BINDING_BEHAVIOR
} from '../../shared/formdef';

export class ScopeDetailSlot implements Slot {
  public static KEY = 'ScopeDetailSlot';

  public key = ScopeDetailSlot.KEY;
  public type = SINGLE_SLOT;
  public title = 'Detail';
  public editors: Editor[];

  public constructor(resources: Array<string>) {
    const resourcesOptions = resources.map((x: string) => {
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
        key: 'resources',
        type: MULTI_SELECT_EDITOR,
        label: 'Resources',
        options: resourcesOptions,
        singleSelection: false,
        bindingBehaviour: VALUE_BINDING_BEHAVIOR,
        allowAddingItems: true
      }
    ];
  }
}
