import {
  Slot,
  SINGLE_SLOT,
  Editor,
  TEXT_EDITOR,
  HIDDEN_EDITOR,
  TEXT_AREA_EDITOR
} from '../../shared/formdef';

export class ScopeDetailSlot implements Slot {
  public static KEY = 'ScopeDetailSlot';

  public key = ScopeDetailSlot.KEY;
  public type = SINGLE_SLOT;
  public title = 'Detail';
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
      }
    ];
  }
}
