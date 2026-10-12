namespace Perpetua.Structurizr.Tests.Fixtures;

/// <summary>A declared container used to exercise assembly scanning in tests.</summary>
[Container("Billing", "InvoiceApi")]
public sealed class InvoiceApi;

[DependsOn("PaymentGateway")]
public sealed class PaymentClient;

[DependsOn("LedgerApi", "Accounting")]
public sealed class LedgerClient;

[DependsOn("AuditTrail", "Compliance", "Records changes")]
public sealed class AuditLogger;

[Event("OrderPlaced")]
public sealed class InvoiceIssuer;

[Consumes("LedgerApi", "PaymentSettled", "Accounting")]
public sealed class PaymentSettlementListener;
