import {
  Slot,
  SINGLE_SLOT,
  Editor,
  HIDDEN_EDITOR,
  TEXT_EDITOR,
  TEXT_AREA_EDITOR
} from '../../shared/formdef';

export class ClaimtypeDetailSlot implements Slot {
  public static KEY = 'ClaimtypeDetailSlot';

  public key = ClaimtypeDetailSlot.KEY;
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
        key: 'description',
        type: TEXT_AREA_EDITOR,
        label: 'Description',
        maxLength: 500
      }
    ];
  }
}
