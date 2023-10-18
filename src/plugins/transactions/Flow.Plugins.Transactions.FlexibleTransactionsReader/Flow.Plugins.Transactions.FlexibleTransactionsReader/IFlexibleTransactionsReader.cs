using Flow.Domain.Transactions;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.Plugins.Contract;

namespace Flow.Plugins.Transactions.FlexibleTransactionsReader;

public interface IFlexibleTransactionsReader : IPlugin, IFormatSpecificReader<(Transaction, Overrides?)> { }
