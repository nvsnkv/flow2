﻿using System.Diagnostics.CodeAnalysis;
using Flow.Domain.Transactions.Transfers;

namespace Flow.Infrastructure.IO.Csv;

[SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
internal class TransferRow : TransferKeyRow
{
    public decimal FEE { get; init; }

    public string? CURRENCY { get; init; }

    public string? COMMENT { get; init; }

    public static explicit operator TransferRow(Transfer key)
    {
        return new TransferRow
        {
            SINK = key.Sink.Key,
            SOURCE = key.Source.Key,
            FEE = key.Fee,
            CURRENCY = key.Currency,
            COMMENT = key.Comment ?? string.Empty
        };
    }

}