﻿using Flow.Domain.Transactions;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Application.Transactions.Contract;

public interface ITransferDetector
{
    bool CheckIsTransfer(RecordedTransaction left, RecordedTransaction right);

    Task<Transfer> Create(RecordedTransaction left, RecordedTransaction right, CancellationToken ct);

    DetectionAccuracy Accuracy { get; }
}
