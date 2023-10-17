using Flow.Domain.Transactions;

namespace Flow.Infrastructure.IO.Criteria.Contract;

public interface ITransactionCriteriaParser
{
    CriteriaParserResult<RecordedTransaction> ParseRecordedTransactionCriteria(string input);

    CriteriaParserResult<RecordedTransaction> ParseRecordedTransactionCriteria(IEnumerable<string> input);
}