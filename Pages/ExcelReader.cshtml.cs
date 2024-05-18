using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System.Globalization;
using System.Net.WebSockets;

namespace EventSchedulePro.Pages
{
    public class ExcelReaderModel : PageModel
    {
        private string FileNameExample = "excel";
        [BindProperty]
        public List<List<object>> OutputDat { get; set; }
        public void OnGet()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            var uploadFol = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads";
            var filepat = Path.Combine(uploadFol, FileNameExample);
            if (System.IO.File.Exists(filepat))
            {            
                using (var stream = System.IO.File.Open(filepat, FileMode.Open, FileAccess.Read))
                {
                    var exceldata = new List<List<object>>();
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        do
                        {
                            while (reader.Read())
                            {
                                var rowData = new List<Object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                    rowData.Add(reader.GetValue(i));
                                exceldata.Add(rowData);
                            }

                        } while (reader.NextResult());
                    }
                    ViewData["ExcelData"] = exceldata;
                }
            }
        }

        [HttpPost("UpdateFile")]
        public async Task<IActionResult> OnPostUpdateFileAsync(IFormFile file)
        {
            try
            {
                if (file != null && file.Length > 0)
                {
                    System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                    var uploadFol = $"{Directory.GetCurrentDirectory()}\\wwwroot\\Uploads";

                    if (!Directory.Exists(uploadFol))
                    {
                        Directory.CreateDirectory(uploadFol);
                    }
                    var filepat = Path.Combine(uploadFol, FileNameExample);

                    if (System.IO.File.Exists(filepat))
                    {
                        System.IO.File.Delete(filepat);
                    }
                    
                    using (var stream = new FileStream(filepat, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    using (var stream = System.IO.File.Open(filepat, FileMode.Open, FileAccess.Read))
                    {
                        var exceldata = new List<List<object>>();
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            do
                            {
                                while (reader.Read())
                                {
                                    var rowData = new List<Object>();
                                    for (int i = 0; i < reader.FieldCount; i++)
                                        rowData.Add(reader.GetValue(i));
                                    exceldata.Add(rowData);
                                }

                            } while (reader.NextResult());
                        }
                        ViewData["ExcelData"] = exceldata;
                        saveFileIntoDatabase(exceldata);
                    }

                }
            } catch (Exception ex)
            {
                ViewData["Exceptiona"] = ex.ToString();
            }
            return Page();
        }
        private void saveFileIntoDatabase(List<List<object>> o)
        {
            for (int i=1; i<o.Count; i++)
            {
                //Time = o[i][0];
                //Activity = o[i][1];
                //Notes = o[i][2];
                //Student Leader = o[i][3];
                //Group = o[i][4];
                //Staff = o[i][5];
                //Location = o[i][6];
                //Check Group not exited then insert
                string groupFromExcel = o[i][4]?.ToString();
                if (String.IsNullOrEmpty(groupFromExcel)) groupFromExcel = "All";
                using (MySqlConnection conn = new MySqlConnection("server=104.238.129.230;uid=jinzschedule;pwd=Metmoivl@123.@124!;database=jinzschedule;connect Timeout=30"))
                {
                    conn.Open();
                    {
                        if (!String.IsNullOrEmpty(groupFromExcel))
                        {
                            var groupList = groupFromExcel.Trim().Split(",");
                            foreach (var group in groupList)
                            {
                                var groupName = checkGroupExited(conn, group.Trim());
                                if (string.IsNullOrEmpty(groupName))
                                {
                                    insertGroup(conn, group.Trim());
                                }
                            }
                        }
                    }
                    //Check Staff not exited then insert
                    {
                        if (!String.IsNullOrEmpty(o[i][5]?.ToString()))
                        {
                            var StaffList = o[i][5].ToString().Trim().Split(",");
                            foreach (var staff in StaffList)
                            {
                                var staffName = checkStaffExited(conn, staff.Trim());
                                if (string.IsNullOrEmpty(staffName))
                                {
                                    insertStaff(conn, staff.Trim());
                                }
                            }
                        }
                    }
                    //Insert Staff into new Group
                    {
                        if (!String.IsNullOrEmpty(groupFromExcel) && !String.IsNullOrEmpty(o[i][5]?.ToString()))
                        {
                            var groupList = groupFromExcel.Trim().Split(",");
                            var StaffList = o[i][5].ToString().Trim().Split(",");
                            List<Group> Groups = getListGroup(conn);
                            List<Staff> Staffs= getListStaff(conn);
                            string GroupIds = "";
                            string GroupNames = "";
                            string StaffIds = "";
                            string StaffNames = "";
                            foreach(var item in groupList) {
                                var group = Groups.FirstOrDefault(x => x.name == item.Trim());
                                if (group != null)
                                {
                                    GroupIds += group.id + ",";
                                    GroupNames += group.name + ",";
                                }
                            }

                            foreach (var item in StaffList)
                            {
                                var staff = Staffs.FirstOrDefault(x => x.username == item.Trim());
                                if (staff != null)
                                {
                                    StaffIds += staff.id + ",";
                                    StaffNames += staff.username + ",";
                                }
                            }

                            if (!string.IsNullOrEmpty(GroupIds) && !string.IsNullOrEmpty(GroupNames))
                            foreach (var staff in StaffList)
                            {
                                updatetStaffGroup(conn, staff.Trim(), GroupIds, GroupNames);
                            }
                            var time = o[i][0].ToString().Trim().Split("-");
                            string starttime = "";
                            string endtime = "";
                            for (int j=0; j<time.Length; j++)
                            {
                                var timeRaw = time[j].Trim();
                                try
                                {
                                    DateTime timeParse = DateTime.ParseExact(timeRaw, "h:mmtt", CultureInfo.InvariantCulture);
                                    if (j == 0)
                                        starttime = timeParse.ToString("HH:mm");
                                    else endtime = timeParse.ToString("HH:mm");
                                } catch(Exception) { }
                            }

                            foreach (var item in groupList)
                            {
                                var group = Groups.FirstOrDefault(x => x.name == item.Trim());
                                if (group != null)
                                {
                                    insertSchedule(conn, o[i][1]?.ToString(), group.id.ToString(), o[i][1]?.ToString(), StaffIds, o[i][3]?.ToString(), o[i][2]?.ToString(), o[i][6]?.ToString(), starttime, endtime, StaffNames);
                                }
                            }

                        }   
                    }
                    //Insert Schedule
                }
            }
        }
        #region Check Group Exited / Add New Group
        private string checkGroupExited(MySqlConnection conn, string name)
        {
            string groupname = "";
            string sql = $"SELECT name FROM groupschedule WHERE name = N'{name}'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    groupname += @rdr.GetString(0) + ", ";
                }
            }
            return groupname;
        }
        private void insertGroup(MySqlConnection conn, string name)
        {
            string myInsertQuery = $"insert into groupschedule(name,detail) values (N'{name}',N'{name}')";
            MySqlCommand myCommand = new MySqlCommand(myInsertQuery, conn);
            myCommand.ExecuteNonQuery();
        }
        #endregion Check Group Exited / Add New Group

        #region Check Staff Exited / Add New Staff
        private string checkStaffExited(MySqlConnection conn, string name)
        {
            string staffname = "";
            string sql = $"SELECT username FROM user WHERE username = N'{name}'";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    staffname += @rdr.GetString(0) + ", ";
                }
            }
            return staffname;
        }
        private void insertStaff(MySqlConnection conn, string username)
        {
            string myInsertQuery = $"INSERT INTO user (username,groupid,roleuser,passwordhash) VALUES ('{username}',0,1,'{Base64Encode("1")}')";
            MySqlCommand myCommand = new MySqlCommand(myInsertQuery, conn);
            myCommand.ExecuteNonQuery();
        }
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        #endregion Check Staff Exited / Add New Staff

        #region Get List Group and Staff
        private List<Group> getListGroup(MySqlConnection conn)
        {
            List<Group> Groups = new List<Group>();
            string sql = $"SELECT id,name FROM groupschedule";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    Groups.Add(new Group{ id = @rdr.GetInt32(0), name = @rdr.GetString(1) });
                }
            }
            return Groups;
        }
        private List<Staff> getListStaff(MySqlConnection conn)
        {
            List<Staff> staffs = new List<Staff>();
            string sql = $"SELECT id,username FROM user";
            MySqlCommand cmd = new MySqlCommand(sql, conn);
            using (MySqlDataReader rdr = cmd.ExecuteReader())
            {
                while (rdr.Read())
                {
                    staffs.Add(new Staff { id = @rdr.GetInt32(0), username = @rdr.GetString(1) });
                }
            }
            return staffs;
        }
        public class Group
        {
            public string name;
            public int id;
        }
        public class Staff
        {
            public string username;
            public int id;
        }
        private void updatetStaffGroup(MySqlConnection conn, string username, string Ids, string Names)
        {
            string myInsertQuery = $"update user set GroupIds ='{Ids}', GroupNames=N'{Names}' where username = '{username}'";
            MySqlCommand myCommand = new MySqlCommand(myInsertQuery, conn);
            myCommand.ExecuteNonQuery();
        }
        #endregion Get List Group and Staff
        private void insertSchedule(MySqlConnection conn, string name,string groupId, string Activity, string staffList, string StudentLeader, string Note, string Location, string from, string to, string staffName)
        {
            string myInsertQuery = $"insert into schedule(name,date,groupid,activity,staff,studentLeader,Note,Location,fromtime,totime,StaffNames) values ('{name}','{DateTime.Now.ToString("yyyy-MM-dd")}',{groupId},N'{Activity}',N'{staffList}',N'{StudentLeader}',N'{Note}',N'{Location}',N'{from}',N'{to}',N'{staffName}')";
            MySqlCommand myCommand = new MySqlCommand(myInsertQuery, conn);
            myCommand.ExecuteNonQuery();
        }
    }

}
