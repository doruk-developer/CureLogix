using CureLogix.Business.Abstract;
using CureLogix.DataAccess.Abstract;
using CureLogix.Entity.Concrete;

namespace CureLogix.Business.Concrete
{
    public class CouncilVoteManager : GenericManager<CouncilVote>, ICouncilVoteService
    {
        public CouncilVoteManager(IGenericRepository<CouncilVote> repository) : base(repository) { }
    }
}