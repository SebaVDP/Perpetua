# Failures

A failed run generates nothing and exits with code `1` (success is `0`). It reports every problem it found, one per line, as a code and a message that names the offending value and the class that declares it, for example:

```
PSTRUCT.0004: Dependency name 'Payment.Gateway' on class 'Billing.PaymentClient' is not a valid identifier.
```

Here `Payment.Gateway` contains a `.`, which a valid identifier cannot (see [Rules](attributes.md#rules)).

| Code | Problem |
|------|---------|
| `PSTRUCT.0001` | A deployment unit declares more than one container. |
| `PSTRUCT.0002` | The container name is not a valid identifier. |
| `PSTRUCT.0003` | The context is not a valid identifier. |
| `PSTRUCT.0004` | A dependency name is not a valid identifier. |
| `PSTRUCT.0005` | A dependency context is not a valid identifier. |
| `PSTRUCT.0006` | A dependency is declared twice with the same context and description. |
| `PSTRUCT.0007` | The same event is published twice in one deployment unit. |
| `PSTRUCT.0008` | An event name is not a valid identifier. |
| `PSTRUCT.0009` | The event named by a `[Consumes]` is not a valid identifier. |
| `PSTRUCT.0010` | The publisher named by a `[Consumes]` is not a valid identifier. |

What a valid identifier looks like is described in [Rules](attributes.md#rules).
