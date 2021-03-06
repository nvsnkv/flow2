using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Contract;

public interface ITransactionCriteriaParser
{
    CriteriaParserResult<RecordedTransaction> ParseRecordedTransactionCriteria(string input);

    CriteriaParserResult<RecordedTransaction> ParseRecordedTransactionCriteria(IEnumerable<string> input);
}