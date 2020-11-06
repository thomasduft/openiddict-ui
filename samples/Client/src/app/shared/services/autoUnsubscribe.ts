export function AutoUnsubscribe(constructor: any) {
  const original = constructor.prototype.ngOnDestroy;

  constructor.prototype.ngOnDestroy = function() {
    // tslint:disable-next-line:forin
    for (const prop in this) {
      const property = this[prop];
      if (property && typeof property.unsubscribe === 'function') {
        property.unsubscribe();
      }
    }

    // tslint:disable-next-line:no-unused-expression
    original &&
      typeof original === 'function' &&
      original.apply(this, arguments);
  };
}
