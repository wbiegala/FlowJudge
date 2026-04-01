namespace FlowJudge.API.Service.Auth
{
    public sealed record JwtConfiguration
    {
        public required string Issuer { get; init; }
        public required string Audience { get; init; }
        public required string MetadataAddress { get; init; }
        public bool RequireHttpsMetadata { get; init; } = false;
        public required string PublicKey { get; init; }
    }
}
