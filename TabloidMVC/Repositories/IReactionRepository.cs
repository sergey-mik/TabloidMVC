using TabloidMVC.Models;
using System.Collections.Generic;

namespace TabloidMVC.Repositories
{
    public interface IReactionRepository
    {
        void CreateReaction(Reaction r);
        List<Reaction> GetAll();

    }
}
