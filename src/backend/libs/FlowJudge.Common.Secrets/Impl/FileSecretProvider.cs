namespace FlowJudge.Common.Secrets.Impl
{
    internal sealed class FileSecretProvider : ISecretProvider
    {
        private readonly string _filePath;

        public FileSecretProvider(string filePath)
        {
            _filePath = filePath;
        }

        public Task<string?> GetSecretAsync(CancellationToken cancellationToken = default)
        {
            var privateKey = File.ReadAllText(_filePath);

            return Task.FromResult<string?>(privateKey);
        }
    }
}
