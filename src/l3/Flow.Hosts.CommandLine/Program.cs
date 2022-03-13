using System.CommandLine;


var root = new RootCommand("Flow CLI")
{
    new Command("accounting", "Transactions management")
    {
        new Command("list"),
        new Command("add"),
        new Command("update"),
        new Command("delete"),
        new Command("find-duplicates")
    },
    new Command("build", "Money flow analysis"),
    new Command("rates", "Exchange rates handling"),
    new Command("transfers", "Transfers adjustment")
};

return await root.InvokeAsync(args);