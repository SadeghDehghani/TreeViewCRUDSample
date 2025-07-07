using Microsoft.AspNetCore.Mvc;
using TreeViewCRUDSample.Models;

namespace TreeViewCRUDSample.Controllers
{


    public class TreeController : Controller
    {
        private readonly AppDbContext _context;

        public TreeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index() => View();

        [HttpGet]
        public JsonResult GetNodes()
        {
            var nodes = _context.TreeNodes
                .Where(n => !n.IsDeleted)
                .OrderBy(n => n.SortOrder)
                .ToList();
            return Json(nodes);
        }

        [HttpPost]
        public async Task<IActionResult> RenameNode(int id, string newName)
        {
            var node = await _context.TreeNodes.FindAsync(id);
            if (node != null)
            {
                node.Name = newName;
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNode(int id)
        {
            var node = await _context.TreeNodes.FindAsync(id);
            if (node != null)
            {
                MarkNodeAsDeleted(id);
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        private void MarkNodeAsDeleted(int nodeId)
        {
            var node = _context.TreeNodes.Find(nodeId);
            if (node != null)
            {
                node.IsDeleted = true;
                var children = _context.TreeNodes.Where(n => n.ParentId == nodeId).ToList();
                foreach (var child in children)
                {
                    MarkNodeAsDeleted(child.Id);
                }
            }
        }

        [HttpPost]
        public async Task<JsonResult> CreateNode(int parentId, string name)
        {
            var siblings = _context.TreeNodes.Where(n => n.ParentId == parentId && !n.IsDeleted);
            int nextOrder = siblings.Any() ? siblings.Max(n => n.SortOrder) + 1 : 1;

            var newNode = new TreeNodeModel
            {
                ParentId = parentId == 0 ? null : parentId,
                Name = name,
                SortOrder = nextOrder,
                IsDeleted = false
            };
            _context.TreeNodes.Add(newNode);
            await _context.SaveChangesAsync();
            return Json(newNode);
        }

        [HttpPost]
        public async Task<IActionResult> MoveNode(int id, int newParentId, int[] siblingOrder)
        {
            var node = await _context.TreeNodes.FindAsync(id);
            if (node != null)
            {
                node.ParentId = newParentId == 0 ? null : newParentId;
                await _context.SaveChangesAsync();

                for (int i = 0; i < siblingOrder.Length; i++)
                {
                    var sibling = await _context.TreeNodes.FindAsync(siblingOrder[i]);
                    if (sibling != null)
                    {
                        sibling.SortOrder = i + 1;
                    }
                }
                await _context.SaveChangesAsync();
            }
            return Ok();
        }

        [HttpPost]
        public IActionResult SaveSelections([FromBody] List<int> selectedIds)
        {
            List<UserPermisson> users = new List<UserPermisson>();
            foreach (var item in selectedIds)
            {

                var CheckedDuplicate = _context
              .UserPermissons.Where(x => x.TreeNodeModelId == item).FirstOrDefault();

                if (CheckedDuplicate == null)
                {
                    var newNode = new UserPermisson
                    {
                        TreeNodeModelId = item,
                    };
                    users.Add(newNode);
                }
            }

          

            _context.UserPermissons.AddRange(users);
            _context.SaveChanges();
            return Ok(new { count = selectedIds.Count, saved = true });

            // return Ok(new { count = selectedIds.Count, saved = true });
        }
    }

}
