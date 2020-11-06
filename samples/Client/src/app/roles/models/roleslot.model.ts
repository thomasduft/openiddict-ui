import {
  Slot,
  SINGLE_SLOT,
  Editor,
  HIDDEN_EDITOR,
  TEXT_EDITOR
} from '../../shared/formdef';

export class RoleDetailSlot implements Slot {
  public static KEY = 'RoleDetailSlot';

  public key = RoleDetailSlot.KEY;
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
      }
    ];
  }
}
