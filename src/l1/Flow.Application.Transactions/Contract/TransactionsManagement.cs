using Autofac;
using Flow.Application.Transactions.Validation;

namespace Flow.Application.Transactions.Contract;

public class TransactionsManagement : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<Accountant>().AsImplementedInterfaces();
        builder.RegisterType<AccountValidator>().AsImplementedInterfaces();
        builder.RegisterType<TransactionValidator>().AsImplementedInterfaces();
        builder.RegisterType<RecordedTransactionValidator>().AsImplementedInterfaces();

        base.Load(builder);
    }
}