﻿using Flow.Domain.Transactions;

namespace Flow.Application.Transactions.Transfers;

internal class ImmediateTransfersDetector : TransferDetectorBase
{
    public override bool CheckIsTransfer(RecordedTransaction left, RecordedTransaction right)
    {
        return left.Timestamp == right.Timestamp
               && left.Amount == -1 * right.Amount
               && left.Amount < 0
               && left.Currency == right.Currency
               && left.Category == right.Category
               && left.Title == right.Title
               && left.Account != right.Account;
    }
    
    public ImmediateTransfersDetector() : base("Immediate transfer")
    {
    }
}