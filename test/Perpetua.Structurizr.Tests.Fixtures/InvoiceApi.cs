namespace Perpetua.Structurizr.Tests.Fixtures;

/// <summary>A declared container used to exercise assembly scanning in tests.</summary>
[Container("InvoiceApi", "Billing")]
public sealed class InvoiceApi;

[DependsOn("PaymentGateway")]
public sealed class PaymentClient;

[DependsOn("LedgerApi", "Accounting")]
public sealed class LedgerClient;

[DependsOn("AuditTrail", "Compliance", "Records changes")]
public sealed class AuditLogger;

[Publishes("OrderPlaced")]
public sealed class InvoiceIssuer;

[Consumes("PaymentSettled", publisherContainer: "LedgerApi", publisherContext: "Accounting")]
public sealed class PaymentSettlementListener;
