export abstract class MessageBase {
  public abstract getType(): string;
}

export interface IMessageSubscriber<T extends MessageBase> {
  onMessage(message: T): void;
  getType(): string;
}

export enum StatusLevel {
  Success,
  Info,
  Warning,
  Danger
}

export class StatusMessage extends MessageBase {
  public static KEY = 'StatusMessage';
  private _hasAction = false;
  private _viewed = false;

  public get viewed(): boolean {
    return this._viewed;
  }
  public set viewed(v: boolean) {
    this._viewed = v;

    if (this._viewed && this.hasAction) {
      delete this.action;
    }
  }

  public action: Function = undefined;
  public get hasAction(): boolean {
    return this._hasAction;
  }

  public constructor(
    public title: string,
    public message: string,
    public level?: StatusLevel,
    action?: Function
  ) {
    super();

    this.level = level !== undefined ? level : StatusLevel.Info;

    if (action) {
      this._hasAction = true;
      this.action = action;
    }
  }

  public getType(): string {
    return StatusMessage.KEY;
  }
}

export interface IdentityResult {
  succeeded: boolean;
  errors: Array<string>;
}

export class ResponseErrorHandler {
  public static handleError(error: Response): any {
    console.log(error || 'Server error');
    return error;
  }
}
