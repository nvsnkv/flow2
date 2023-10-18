rm ../plugin-deps/*
dotnet pack ../../src/l0/Flow.Domain.Common/Flow.Domain.Common.csproj -c Release -o ../plugin-deps
dotnet pack ../../src/l0/Flow.Domain.Patterns/Flow.Domain.Patterns.csproj -c Release -o ../plugin-deps
dotnet pack ../../src/l0/Flow.Domain.Transactions/Flow.Domain.Transactions.csproj -c Release -o ../plugin-deps
dotnet pack ../../src/l1/Flow.Application.Transactions.Contract/Flow.Application.Transactions.Contract.csproj -c Release -o ../plugin-deps
dotnet pack ../../src/l2/Flow.Infrastructure.Configuration/Flow.Infrastructure.Configuration.csproj -c Release -o ../plugin-deps
dotnet pack ../../src/l2/IO/Flow.Infrastructure.IO.Contract/Flow.Infrastructure.IO.Contract.csproj -c Release -o ../plugin-deps
dotnet pack ../../src/l2/Flow.Infrastructure.Plugins.Contract/Flow.Infrastructure.Plugins.Contract.csproj -c Release -o ../plugin-deps
