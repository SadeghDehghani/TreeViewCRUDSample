using System.ComponentModel.DataAnnotations;

namespace TreeViewCRUDSample.Models
{
    public class TreeNodeModel
    {
        [Key]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }

        public int SortOrder { get; set; }
        public bool IsDeleted { get; set; }
    }
}
