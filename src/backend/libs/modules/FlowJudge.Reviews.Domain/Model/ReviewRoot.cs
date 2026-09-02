using FlowJudge.Common.Domain;

namespace FlowJudge.Reviews.Domain.Model
{
    public sealed class ReviewRoot : AggregateRoot
    {

        public RepositoryEntity Repository { get; private set; }
        public ReviewStatus Status { get; private set; }
        
    }
}
