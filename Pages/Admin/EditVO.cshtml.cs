using EventSchedulePro.Data.Class;
using EventSchedulePro.Data.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EventSchedulePro.Pages.Admin
{
    public class EditVOModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            public string MultiHTML { get; set; }
        }

        public readonly EventDBContext _context;
        public EditVOModel(EventDBContext context)
        {
            _context = context;
        }
        public IActionResult OnGet()
        {
            var userName = HttpContext.Session.GetString("AdminUserName");
            if (string.IsNullOrEmpty(userName))
            {
                return new RedirectToPageResult("/Index");
            }
            if (Input == null)
            {
                Input = new InputModel();
            }
            var IPOContext = _context.Contents.OrderByDescending(x => x.CreateTime).FirstOrDefault(x => x.Type.CompareTo("VO") == 0);
            if (IPOContext != null)
            {
                Input.MultiHTML = IPOContext.ContentHTML;
            }

            return Page();
        }
        [HttpPost("FindStaff")]
        public async Task<IActionResult> OnPostFindStaffAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (String.IsNullOrEmpty(Input.MultiHTML))
            {
                ModelState.AddModelError(string.Empty, "Please input any content");
                return Page();
            }
            var htmlraw = Input.MultiHTML;
            DateTime localDateTime = DateTime.Now;

            // Convert local DateTime to UTC
            DateTime utcDateTime = localDateTime.ToUniversalTime();
            Content t = new Content { Name = "IPO Raw", Type = "VO", ContentHTML = htmlraw, CreateTime = utcDateTime };
            _context.Contents.Add(t);
            await _context.SaveChangesAsync();
            return Page();
        }
    }

}
