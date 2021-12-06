using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Contract;

public interface ITransactionCriteriaParser
{
    [Obsolete("Use ParseRecordedTransactionCriteria(string) instead")]
    CriteriaParserResult<RecordedTransaction> ParseRecordedTransactionCriteria(IEnumerable<string> parts);

    CriteriaParserResult<RecordedTransaction> ParseRecordedTransactionCriteria(string input);
}