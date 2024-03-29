﻿using Flow.Domain.Transactions;
using Flow.Infrastructure.Configuration.Contract;
using Flow.Infrastructure.IO.Generics;
using Flow.Infrastructure.IO.Transactions.Contract;

namespace Flow.Infrastructure.IO.Transactions;

internal class DefaultJsonTransactionsSerializer : ITransactionsReader, ITransactionsWriter
{
    private readonly JsonSerializer json;

    public DefaultJsonTransactionsSerializer(JsonSerializer json)
    {
        this.json = json;
    }

    public Task<IEnumerable<IncomingTransaction>> ReadTransactions(StreamReader reader, CancellationToken ct)
    {
        return json.Read(reader, (JsonTransaction j) => ((Transaction)j, (Overrides?)null));
    }

    public Task<IEnumerable<RecordedTransaction>> ReadRecordedTransactions(StreamReader reader, CancellationToken ct)
    {
        return json.Read(reader, (JsonRecordedTransaction j) => (RecordedTransaction)j);
    }

    public Task WriteTransactions(StreamWriter writer, IEnumerable<Transaction> transactions, CancellationToken ct)
    {
        return json.Write(writer, transactions, ct);
    }

    public Task WriteRecordedTransactions(StreamWriter writer, IEnumerable<RecordedTransaction> transactions, CancellationToken ct)
    {
        return json.Write(writer, transactions, ct);
    }

    public OldSupportedFormat Format => OldSupportedFormat.JSON;
}
