### Checklist

1. Use the BEM naming convention (More Infos & Documentation -> [Get BEM](http://getbem.com/))
```css
.block {
  /* ... */
  &__element {
    /* ... */
    &--modifier {
      /* ... */
    }
  }

  &--modifier {
    /* ... */
  }
}
```

2. Split materials (UI blocks) into:
- atoms
- molecules
- organisms
- templates
- pages 

> Atomic Design Pattern
