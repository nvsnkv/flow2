using System.Text;
using Flow.Application.Transactions.Contract;
using Flow.Infrastructure.IO.Collections;
using Flow.Infrastructure.IO.Contract;
using Flow.Infrastructure.IO.CSV.Contract;
using Flow.Infrastructure.IO.Import.Contract;

namespace Flow.Infrastructure.IO.Import;

class FileReader : IFileReader
{
    private readonly IReaders<IncomingTransaction> readers;

    public FileReader(IReaders<IncomingTransaction> readers)
    {
        this.readers = readers;
    }

    public async Task<IEnumerable<IncomingTransaction>> ReadFromFile(string path, CancellationToken ct)
    {
        var parts = path.Split('.').Reverse().Skip(1).ToList();
        var encoding = Encoding.UTF8;
        var format = CSVIO.SupportedFormatName;

        if (parts.Count > 1)
        {
            format = parts[0];
        }

        if (parts.Count > 2)
        {
            encoding = Encoding.GetEncoding(parts[1]);
        }

        using var reader = new StreamReader(File.OpenRead(path), encoding);

        return await readers.GetFor(new SupportedFormat(format)).Read(reader, ct);
    }
}
