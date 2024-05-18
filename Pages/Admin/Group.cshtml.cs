using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;

namespace EventSchedulePro.Pages.Admin
{
    public class GroupModel : PageModel
    {        /// <summary>
             ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
             ///     directly from your code. This API may change or be removed in future releases.
             /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string ErrorMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            public string Username { get; set; }
            [Required]
            public List<string> GroupList { get; set; }
        }
        public IActionResult OnGet()
        {
            var userName = HttpContext.Session.GetString("AdminUserName");
            if (string.IsNullOrEmpty(userName))
            {
                return new RedirectToPageResult("/Admin/Login");
            }
            return Page();
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            if (Input.Username.CompareTo("Select user") != 0)
                using (MySqlConnection conn = new MySqlConnection("server=104.238.129.230;uid=jinzschedule;pwd=Metmoivl@123.@124!;database=jinzschedule;connect Timeout=30"))
            {
                    string GroupIdList = (Input.GroupList != null && Input.GroupList.Any()) ? string.Join(",", Input.GroupList) : "";
                    if (string.IsNullOrEmpty(GroupIdList)) return Page();
                    string GroupName = !string.IsNullOrEmpty(GroupIdList) ? getGroupName(GroupIdList) : "";
                    conn.Open();
                string myInsertQuery = $"update user set GroupIds ='{GroupIdList}', GroupNames=N'{GroupName}' where id = '{Input.Username}'";
                MySqlCommand myCommand = new MySqlCommand(myInsertQuery, conn);
                myCommand.ExecuteNonQuery();
            }
            return Page();
        }
        private string getGroupName(string listId)
        {
            string groupname = "";
            using (MySqlConnection conn = new MySqlConnection("server=104.238.129.230;uid=jinzschedule;pwd=Metmoivl@123.@124!;database=jinzschedule;connect Timeout=30"))
            {
                conn.Open();
                string sql = $"SELECT name FROM groupschedule where id in ({listId})";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        groupname += @rdr.GetString(0) + ", ";

                    }
                }
            }
            return groupname;
        }
    }
}
