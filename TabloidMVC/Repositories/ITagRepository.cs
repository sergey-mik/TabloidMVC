using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ITagRepository
    {

        public List<Tag> GetAllTags();
        public Tag GetTagById(int id);
        public void AddTag(Tag tag);
        public void UpdateTag(Tag tag);
        public void DeleteTag(int id);

    }
}
