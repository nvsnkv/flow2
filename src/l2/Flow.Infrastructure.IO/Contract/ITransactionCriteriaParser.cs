using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Contract;

public interface ITransactionCriteriaParser
{
    CriteriaParserResult<Transaction> ParseTransactionCriteria(IEnumerable<string> parts);

    CriteriaParserResult<RecordedTransaction> ParseRecordedTransactionCriteria(IEnumerable<string> parts);
}