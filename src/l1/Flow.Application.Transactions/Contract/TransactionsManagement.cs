using System.Runtime.CompilerServices;
using Autofac;
using Flow.Application.Transactions.Transfers;
using Flow.Application.Transactions.Validation;

[assembly:InternalsVisibleTo("Flow.Application.Transactions.UnitTests")]

namespace Flow.Application.Transactions.Contract;

public class TransactionsManagement : Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterType<AccountValidator>().AsImplementedInterfaces();
        builder.RegisterType<TransactionValidator>().AsImplementedInterfaces();
        builder.RegisterType<RecordedTransactionValidator>().AsImplementedInterfaces();

        builder.RegisterType<TransferKeysValidator>().AsImplementedInterfaces();
        builder.RegisterType<ImmediateTransfersDetector>().AsImplementedInterfaces();
        builder.RegisterType<FuzzyTransferDetector>().AsImplementedInterfaces();
        
        builder.RegisterType<Accountant>().AsImplementedInterfaces();

        base.Load(builder);
    }
}