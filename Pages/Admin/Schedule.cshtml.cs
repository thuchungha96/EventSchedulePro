using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Runtime.Intrinsics.Arm;

namespace EventSchedulePro.Pages.Admin
{
    public class ScheduleModel : PageModel
    {
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
            /// 


            public string Id { get; set; }
            public string Group { get; set; }

            [Required]
            public string Name { get; set; }
            public string Activity { get; set; }
            public string Staff { get; set; }
            public string Leader { get; set; }
            public string Note { get; set; }
            public string Location { get; set; }
            public string TimeSchedule { get; set; }
            public string FromTime { get; set; }
            public string ToTime { get; set; }
            public List<String> Stafff { get; set; }
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
        [HttpPost("Add")]
        public async Task<IActionResult> OnPostAddAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            try
            {
                if (Input.Group.CompareTo("Select group") != 0)
                    if (String.IsNullOrEmpty(Input.Id)) { }
                using (MySqlConnection conn = new MySqlConnection("server=104.238.129.230;uid=jinzschedule;pwd=Metmoivl@123.@124!;database=jinzschedule;connect Timeout=30"))
                {
                    conn.Open();
                    string myInsertQuery = "";
                    string stafidList = (Input.Stafff != null && Input.Stafff.Any()) ? string.Join(",", Input.Stafff) : "";
                    string staffname = !string.IsNullOrEmpty(stafidList) ? getStaffName(stafidList) : "";
                    if (String.IsNullOrEmpty(Input.Id))
                    {
                        myInsertQuery = $"insert into schedule(name,date,groupid,activity,staff,studentLeader,Note,Location,fromtime,totime,StaffNames) values ('{Input.Name}','{DateTime.Parse(Input.TimeSchedule).ToString("yyyy-MM-dd")}',{Input.Group},N'{Input.Activity}',N'{stafidList}',N'{Input.Leader}',N'{Input.Note}',N'{Input.Location}',N'{Input.FromTime}',N'{Input.ToTime}',N'{staffname}')";
                    }
                    else
                    {
                        CultureInfo enUS = new CultureInfo("en-US");
                        DateTime dateValue;
                        if (DateTime.TryParseExact(Input.TimeSchedule, "dd/MM/yyyy", enUS,
                                  DateTimeStyles.None, out dateValue))
                        {
                            myInsertQuery = $"update schedule set name=N'{Input.Name}', date='{dateValue.ToString("yyyy-MM-dd")}', groupid={Input.Group},activity = N'{Input.Activity}', staff =N'{stafidList}', studentLeader=N'{Input.Leader}', Note= N'{Input.Note}', Location= N'{Input.Location}',fromtime =N'{Input.FromTime}',totime=N'{Input.ToTime}',StaffNames=N'{staffname}' where id = {Input.Id}   ";
                        }
                    }
                    MySqlCommand myCommand = new MySqlCommand(myInsertQuery, conn);
                    myCommand.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {

            }
            return Page();
        }
        [HttpPost("Delete")]
        public async Task<IActionResult> OnPostDeleteAsync(string Id = null)
        {
           if (Id != null)
            {
                try
                {
                    int id = int.Parse(Id);
                    using (MySqlConnection conn = new MySqlConnection("server=104.238.129.230;uid=jinzschedule;pwd=Metmoivl@123.@124!;database=jinzschedule;connect Timeout=30"))
                    {
                        conn.Open();
                        string myInsertQuery = $"delete from schedule where id = {id}";
                        MySqlCommand myCommand = new MySqlCommand(myInsertQuery, conn);
                        myCommand.ExecuteNonQuery();
                    }

                }
                catch(Exception ex) { }
            }
            return Page();
        }
        [HttpPost("Edit")]
        public async Task<IActionResult> OnPostEditAsync(string Id = null)
        {
            if (Id != null)
            {
                using (MySqlConnection conn = new MySqlConnection("server=104.238.129.230;uid=jinzschedule;pwd=Metmoivl@123.@124!;database=jinzschedule;connect Timeout=30"))
                {
                    conn.Open();
                    string sql = $"select us.id, us.name, us.Activity, us.Staff, us.StudentLeader, us.Note, us.Date, us.Location, CASE WHEN gd.id IS NULL THEN '' ELSE gd.id END AS name, us.fromtime, us.totime from schedule us left join GroupSchedule gd on us.groupid = gd.id where us.id ={Id}";
                    MySqlCommand cmd = new MySqlCommand(sql, conn);
                    using (MySqlDataReader rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            Input.Id = Id;
                            Input.Name = @rdr.GetString(1);
                            Input.Group = @rdr.GetString(8);
                            Input.Activity = @rdr.GetString(2);
                            Input.Stafff = @rdr.GetString(3) != null ? new List<string>(@rdr.GetString(3).ToString().Split(",")) : new List<string>();
                            Input.Leader = @rdr.GetString(4);
                            Input.Note = @rdr.GetString(5);
                            Input.Location = @rdr.GetString(7);
                            Input.TimeSchedule = @rdr.GetDateTime(6).ToString("dd/MM/yyyy");
                            Input.FromTime = @rdr.GetString(9);
                            Input.ToTime = @rdr.GetString(10);

                        }
                    }
                }
            }
            return Page();
        }
        private string getStaffName(string listId)
        {
            string username = "";
            using (MySqlConnection conn = new MySqlConnection("server=104.238.129.230;uid=jinzschedule;pwd=Metmoivl@123.@124!;database=jinzschedule;connect Timeout=30"))
            {
                conn.Open();
                string sql = $"SELECT Username FROM user where id in ({listId})";
                MySqlCommand cmd = new MySqlCommand(sql, conn);
                using (MySqlDataReader rdr = cmd.ExecuteReader())
                {
                    while (rdr.Read())
                    {
                        username+= @rdr.GetString(0) +", ";

                    }
                }
            }
            return username;
        }
    }
}
