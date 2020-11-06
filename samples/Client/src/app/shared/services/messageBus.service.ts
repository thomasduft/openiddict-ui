import { Injectable } from '@angular/core';

import { MessageBase, IMessageSubscriber } from './models';

@Injectable({
  providedIn: 'root'
})
export class MessageBus {
  private subscribers: { [id: number]: IMessageSubscriber<MessageBase>; };

  public constructor() {
    this.subscribers = {};
  }

  public subsribe<T extends MessageBase>(subscriber: IMessageSubscriber<T>): number {
    const id = new Date().valueOf();
    this.subscribers[id] = subscriber;
    return id;
  }

  public unsubscribe(id: number): void {
    delete this.subscribers[id];
  }

  public publish<T extends MessageBase>(message: T): void {
    const subscribersForMessage = this.getSubscribersForMessage<T>(message);

    if (subscribersForMessage.length === 0) {
      return;
    }

    subscribersForMessage.forEach((subscriberForMessage: IMessageSubscriber<T>) => {
      subscriberForMessage.onMessage(message);
    });
  }

  private getSubscribersForMessage<T extends MessageBase>(message: T): Array<IMessageSubscriber<T>> {
    const subscribers = new Array<IMessageSubscriber<T>>();

    for (const key in this.subscribers) {
      if (this.subscribers.hasOwnProperty(key)) {
        const subscriber = this.subscribers[key];
        if (subscriber.getType() === message.getType()) {
          subscribers.push(subscriber);
        }
      }
    }

    return subscribers;
  }
}
