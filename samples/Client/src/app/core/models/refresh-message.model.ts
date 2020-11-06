import { MessageBase } from '../../shared';

export class RefreshMessage extends MessageBase {
  public static KEY = 'RefreshMessage';

  public readonly source: string;

  public constructor(source: string) {
    super();

    this.source = source;
  }

  public getType(): string {
    return RefreshMessage.KEY;
  }
}
