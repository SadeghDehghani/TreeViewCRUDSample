using System.ComponentModel.DataAnnotations;

namespace TreeViewCRUDSample.Models
{
    public class UserPermisson
    {
        [Key]
        public int Id { get; set; }
        public int TreeNodeModelId { get; set; }
        public virtual TreeNodeModel TreeNodeModel  { get; set; }
    }
}
