using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Security.Principal;
using System.Web;
using System.DirectoryServices.AccountManagement;
using System.Threading;
using System.Web.UI;


namespace Cloud_Tickets.Models
{
    public class Computer
    {
        protected int _id;
        protected string _host;
        protected string _user;
        protected string _ip;
        protected string _report;
        protected string _description;
        protected string _date;
        protected string _location;
        protected string _status;
        protected string _notifications;
        protected bool _isadmin;

        protected int _octet;
        protected int _octetvpn;
        protected string _db_result = "";
        protected string db = "Cloudtickets_db";
        protected string user_db = "user";
        protected string pwd = "";

        public int OCTETVPN
        {
            get { return _octetvpn; }
            set { _octetvpn = value; }
        }

        public string DB_RESULT
        {
            get { return _db_result; }
            set { _db_result = value; }
        }
        public int ID
        {
            get { return _id; }
            set { _id = value; }
        }
        public string Host
        { 
          get { return _host; }
          set { _host = value; }
        }
        public string User
        {
            get { return _user; }
            set { _user = value; }
        }
        public string IP
        {
            get { return _ip; }
            set { _ip = value; }
        }

        public string Report
        {
            get { return _report; }
            set { _report = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
        public string Date
        {
            get { return _date; }
            set { _date = value; }
        }
        public string Location
        {
            get { return _location; }
            set { _location = value; }
        }
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }
        public string Notifications
        {
            get { return _notifications; }
            set { _notifications = value; }
        }
        public bool IsAdmin
        {
            get { return _isadmin; }
            set { _isadmin = value; }
        }

        
        public void GetHostDetails()
        {
            try {
                _ip = GetLanIPAddress();
                _host = DetermineCompName(_ip);
                _date = DateTime.Now.ToString("dd'/'MM'/'yyyy HH:mm:ss");
                _octet = Convert.ToInt32(IPAddress.Parse(GetLanIPAddress()).GetAddressBytes()[2]);
                _octetvpn = Convert.ToInt32(IPAddress.Parse(GetLanIPAddress()).GetAddressBytes()[1]);
                _location = GetLocation();
            }
            catch (Exception ex)
            {
                _db_result = ex.Message;
            }
        }
        public static string DetermineCompName(string IP)
        {
            IPAddress myIP = IPAddress.Parse(IP);
            IPHostEntry GetIPHost = Dns.GetHostEntry(myIP);
            List<string> compName = GetIPHost.HostName.ToString().Split('.').ToList();
            return compName.First();
        }
        private string GetLanIPAddress()
        {
            return HttpContext.Current.Request.UserHostAddress.ToString();

        }
        private string GetLocation()
        {
            string loc;
            switch (_octet)
            {
                case 1:
                    loc = "Sede";
                    break;
                case 3:
                    loc = "Poente";
                    break;
                case 4:
                    loc = "Piscina";
                    break;
                case 5:
                    loc = "Nascente";
                    break;
                default:
                    loc = "";
                    break;
            }
            return loc;
        }
        public int SaveToDB(string user, string host, string ip, string loc, string date, string report, string description)
        {
            int result;
            string connetionString = null;
            SqlConnection cnn ;
            connetionString = "Data Source=.\\HELPDESK;Initial Catalog=" + db + ";User ID=" + user_db + ";Password=" + pwd + "";
            cnn = new SqlConnection(connetionString);
            try
            {
                cnn.Open();
                string ExistUser_query = "SELECT COUNT(*) from dbo.tbl_user_details WHERE username LIKE @user";
                string InsertUser_query = "INSERT INTO dbo.tbl_user_details (username,pc,ip,location,timestamp) VALUES(@username,@hostname,@ip, @location,@date)";
                string InsertReport_query = "INSERT INTO dbo.tbl_assistance_description(Report,Description,FK_IDuser,Date,Status) VALUES (@Report,@Description,(SELECT ID FROM dbo.tbl_user_details WHERE username = @username_),@date_,@status)";
                SqlCommand command = new SqlCommand(ExistUser_query, cnn);
                command.Parameters.AddWithValue("@user", user);
                int userCount = (int)command.ExecuteScalar();
                int userCount = (int)command.ExecuteScalar();
                if (userCount > 0)
                {
                    SqlCommand command2 = new SqlCommand(InsertReport_query, cnn);
                    command2.Parameters.AddWithValue("@username_", user);
                    command2.Parameters.AddWithValue("@Report", report);
                    command2.Parameters.AddWithValue("@Description", description);
                    command2.Parameters.AddWithValue("@date_", Convert.ToDateTime(date));
                    command2.Parameters.AddWithValue("@status", "Aberto");
                    command2.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand command2 = new SqlCommand(InsertUser_query, cnn);
                    SqlCommand command3 = new SqlCommand(InsertReport_query, cnn);
                    command2.Parameters.AddWithValue("@username", user);
                    command2.Parameters.AddWithValue("@hostname", host);
                    command2.Parameters.AddWithValue("@ip", ip);
                    command2.Parameters.AddWithValue("@location", loc);
                    command2.Parameters.AddWithValue("@date", Convert.ToDateTime(date));
                    command3.Parameters.AddWithValue("@username_", user);
                    command3.Parameters.AddWithValue("@Report", report);
                    command3.Parameters.AddWithValue("@Description", description);
                    command3.Parameters.AddWithValue("@date_", Convert.ToDateTime(date));
                    command3.Parameters.AddWithValue("@status", "Aberto");
                    command2.ExecuteNonQuery();
                    command3.ExecuteNonQuery();
                }
                result = 1;
                cnn.Close();
            }
            catch (Exception ex)
            {
                result = 0;
                DB_RESULT = ex.Message;
            }
            return result;
        }
        public void SendEmail(string usr,string in_ch,string descricao,string loc_)
        {
            try { 
            MailMessage mail = new MailMessage();
            SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
            smtpServer.UseDefaultCredentials = false;
            smtpServer.Credentials = new System.Net.NetworkCredential("email@domain.com", "password");
            smtpServer.EnableSsl = true;
            
            smtpServer.Port = 587; // Gmail works on this ports
            smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
            mail.From = new MailAddress("email@domain.com");
            mail.To.Add("email@domain.com");
            mail.To.Add("email@domain.com");
            mail.To.Add(usr + "@domail.pt");
            mail.IsBodyHtml = true;
            mail.Subject = in_ch + " - " + usr + " - " + loc_;
            mail.Body = descricao;
            smtpServer.Send(mail);
            }
            catch (Exception ex)
            {
                _db_result = ex.Message;
            }
        }

        public List<Computer> GetClosedTickets(string username_)
        {
            List<Computer> Listar = new List<Computer>();
            //int result;
            string connetionString = null;
            SqlConnection cnn ;
            connetionString = "Data Source=.\\HELPDESK;Initial Catalog=" + db + ";User ID=" + user_db + ";Password=" + pwd + "";
            cnn = new SqlConnection(connetionString);
            try
            {
                cnn.Open();
                string query = "SELECT * FROM [tbl_user_details] JOIN [tbl_assistance_description] ON [tbl_user_details].username LIKE @user_ AND [tbl_assistance_description].Status LIKE @status_ AND [tbl_assistance_description].FK_IDuser LIKE [tbl_user_details].ID ORDER BY [tbl_assistance_description].Date DESC";
                try
                {
                    SqlCommand command = new SqlCommand(query, cnn);
                    command.Parameters.AddWithValue("@status_", "Fechado");
                    try { 
                        command.Parameters.AddWithValue("@user_", username_);
                        try
                        {
                            SqlDataReader dataReader = command.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                            
                                while (dataReader.Read())
                                {
                                    try {
                                        Listar.Add(new Computer()
                                        {
                                            //ID = dataReader.IsDBNull(0) ? Convert.ToInt32(null) : dataReader.GetInt32(0),
                                            User = dataReader.IsDBNull(1) ? null : dataReader.GetString(1),
                                            Host = dataReader.IsDBNull(2) ? null : dataReader.GetString(2),
                                            IP = dataReader.IsDBNull(3) ? null : dataReader.GetString(3),
                                            Location = dataReader.IsDBNull(4) ? null : dataReader.GetString(4),
                                            ID = dataReader.IsDBNull(6) ? 0 : dataReader.GetInt32(6),
                                            Report = dataReader.IsDBNull(7) ? null : dataReader.GetString(7),
                                            Description = dataReader.IsDBNull(8) ? null : dataReader.GetString(8),
                                            Date = dataReader.GetDateTime(9).ToString(),
                                            Status = dataReader.IsDBNull(10) ? null : dataReader.GetString(10)
                                        });
                                    }
                                    catch (SqlException ex)
                                    {
                                        Listar.Add(new Computer()
                                        {
                                            DB_RESULT = ex.Message
                                        });
                                    }
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            Listar.Add(new Computer()
                            {
                                DB_RESULT = ex.Message
                            });
                        }
                        cnn.Close();
                    }
                    catch (SqlException ex)
                    {
                        Listar.Add(new Computer()
                        {
                            DB_RESULT = ex.Message
                        });
                    }
                }

                catch (SqlException ex)
                {
                    Listar.Add(new Computer()
                    {
                        DB_RESULT = ex.Message
                    });
                }
            }
            
            catch (SqlException ex)
            {
                Listar.Add(new Computer()
                {
                    DB_RESULT = ex.Message
                });
            }
            return Listar;

        }
        public List<Computer> GetOpenTickets(string username_)
        {
            List<Computer> Listar = new List<Computer>();
            string connetionString = null;
            SqlConnection cnn ;
            connetionString = "Data Source=.\\HELPDESK;Initial Catalog=" + db + ";User ID=" + user_db + ";Password=" + pwd + "";
            cnn = new SqlConnection(connetionString);
            try
            {
                cnn.Open();
                string query = "SELECT * FROM [tbl_user_details] JOIN [tbl_assistance_description] ON [tbl_user_details].username LIKE @user_ AND [tbl_assistance_description].Status LIKE @status_ AND [tbl_assistance_description].FK_IDuser LIKE [tbl_user_details].ID ORDER BY [tbl_assistance_description].Date DESC";
                try
                {
                    SqlCommand command = new SqlCommand(query, cnn);
                    command.Parameters.AddWithValue("@status_", "Aberto");
                    try { 
                        command.Parameters.AddWithValue("@user_", username_);
                        try
                        {
                            SqlDataReader dataReader = command.ExecuteReader();
                            if (dataReader.HasRows)
                            {
                            
                                while (dataReader.Read())
                                {
                                    try 
                                    {
                                        Listar.Add(new Computer()
                                        {
                                            User = dataReader.IsDBNull(1) ? null : dataReader.GetString(1),
                                            Host = dataReader.IsDBNull(2) ? null : dataReader.GetString(2),
                                            IP = dataReader.IsDBNull(3) ? null : dataReader.GetString(3),
                                            Location = dataReader.IsDBNull(4) ? null : dataReader.GetString(4),
                                            ID = dataReader.IsDBNull(6) ? 0 : dataReader.GetInt32(6),
                                            Report = dataReader.IsDBNull(7) ? null : dataReader.GetString(7),
                                            Description = dataReader.IsDBNull(8) ? null : dataReader.GetString(8),
                                            Date = dataReader.GetDateTime(9).ToString(),
                                            Status = dataReader.IsDBNull(10) ? null : dataReader.GetString(10)
                                        });
                                    }
                                    catch (SqlException ex)
                                    {
                                        Listar.Add(new Computer()
                                        {
                                        DB_RESULT = ex.Message
                                        });
                                    }
                                }
                            }
                        }
                        catch (SqlException ex)
                        {
                            Listar.Add(new Computer()
                            {
                                DB_RESULT = ex.Message
                            });
                        }
                        cnn.Close();
                    }
                    catch (SqlException ex)
                    {
                        Listar.Add(new Computer()
                        {
                            DB_RESULT = ex.Message
                        });
                    }
                }
                catch (SqlException ex)
                {
                    Listar.Add(new Computer()
                    {
                        DB_RESULT = ex.Message
                    });
                }
            }
            
            catch (SqlException ex)
            {
                Listar.Add(new Computer()
                {
                    DB_RESULT = ex.Message
                });
            }
            return Listar;
            

        }

          public List<Computer> Admin()
          {
              List<Computer> listar = new List<Computer>();
              //int result;
              string connetionString = null;
              SqlConnection cnn;
              connetionString = "Data Source=.\\HELPDESK;Initial Catalog=" + db + ";User ID=" + user_db + ";Password=" + pwd + "";
              cnn = new SqlConnection(connetionString);
              try
              {
                  cnn.Open();
                  string query = "SELECT * FROM [tbl_user_details] JOIN [tbl_assistance_description] ON [tbl_assistance_description].FK_IDuser LIKE [tbl_user_details].ID AND [tbl_assistance_description].Status LIKE @status_ ORDER BY [tbl_assistance_description].Status ASC, [tbl_assistance_description].Date DESC";
                  try
                  {
                      SqlCommand command = new SqlCommand(query, cnn);
                      command.Parameters.AddWithValue("@status_", "Aberto");
                      try
                      {
                          SqlDataReader dataReader = command.ExecuteReader();
                          if (dataReader.HasRows)
                          {
                              while (dataReader.Read())
                              {
                                  try
                                  {
                                      listar.Add(new Computer()
                                      {
                                          User = dataReader.IsDBNull(1) ? null : dataReader.GetString(1),
                                          Host = dataReader.IsDBNull(2) ? null : dataReader.GetString(2),
                                          IP = dataReader.IsDBNull(3) ? null : dataReader.GetString(3),
                                          Location = dataReader.IsDBNull(4) ? null : dataReader.GetString(4),
                                          ID = dataReader.IsDBNull(6) ? 0 : dataReader.GetInt32(6),
                                          Report = dataReader.IsDBNull(7) ? null : dataReader.GetString(7),
                                          Description = dataReader.IsDBNull(8) ? null : dataReader.GetString(8),
                                          Date = dataReader.GetDateTime(9).ToString(),
                                          Status = dataReader.IsDBNull(10) ? null : dataReader.GetString(10)
                                      });
                                  }
                                  catch (SqlException ex)
                                  {
                                      listar.Add(new Computer()
                                      {
                                          DB_RESULT = ex.Message
                                      });
                                  }
                              }
                          }
                      }
                      catch (SqlException ex)
                      {
                          listar.Add(new Computer()
                          {
                              DB_RESULT = ex.Message
                          });
                      }
                      cnn.Close();
                  }
                  catch (SqlException ex)
                  {
                      listar.Add(new Computer()
                      {
                          DB_RESULT = ex.Message
                      });
                  }
              }

              catch (SqlException ex)
              {
                  listar.Add(new Computer()
                  {
                      DB_RESULT = ex.Message
                  });
              }
              return listar;

          }
          public string SendNotify(string Message) {
              _notifications = Message;
              return Message;
          }
          public bool GetAdmin(string username_)
          {
              bool _result_;
              string connetionString = null;
              SqlConnection cnn;
              connetionString = "Data Source=.\\HELPDESK;Initial Catalog=" + db + ";User ID=" + user_db + ";Password=" + pwd + "";
              cnn = new SqlConnection(connetionString);
              try
              {
                  cnn.Open();
                  string query = "SELECT COUNT(*) FROM [tbl_Admin] WHERE Username LIKE @user_";
                  try
                  {
                      SqlCommand command = new SqlCommand(query, cnn);
                      System.Security.Principal.IPrincipal user = System.Web.HttpContext.Current.User;
                      System.Security.Principal.IIdentity identity = user.Identity;
                      command.Parameters.AddWithValue("@user_", username_);
                      try
                      {
                          int userCount = (int)command.ExecuteScalar();
                          if (userCount > 0)
                          {
                              _result_ = true;
                          }
                          else
                          {
                              _result_ = false;
                          }
                      }
                      catch (SqlException ex)
                      {
                          _result_ = false;
                      }
                      cnn.Close();
                  }
                  catch (SqlException ex)
                  {
                      _result_ = false;
                  }
              }
              catch (SqlException ex)
              {
                  _result_ = false;
              }
              return _result_;

          }
          public string ChangeStatus(int _ID, string username)
          {
              string _result_ = null;
              string connetionString = null;
              SqlConnection cnn;
              connetionString = "Data Source=.\\HELPDESK;Initial Catalog=" + db + ";User ID=" + user_db + ";Password=" + pwd + "";
              cnn = new SqlConnection(connetionString);
              try
              {
                  cnn.Open();
                  string query = "UPDATE [tbl_assistance_description] SET [tbl_assistance_description].Status = 'Fechado' FROM [tbl_user_details] UD, [tbl_assistance_description] AD WHERE UD.Username LIKE @user_ AND AD.ID = @ID";
                  try
                  {
                      SqlCommand command = new SqlCommand(query, cnn);
                      command.Parameters.AddWithValue("@user_", username);
                      command.Parameters.AddWithValue("@ID", _ID);
                      try
                      {
                          SqlDataReader dataReader = command.ExecuteReader();
                          _result_ = "true";
                      }
                      catch (SqlException ex)
                      {
                          _result_ = ex.Message;
                      }
                      cnn.Close();
                  }
                  catch (SqlException ex)
                  {
                      _result_ = ex.Message;
                  }
              }
              catch (SqlException ex)
              {
                  _result_ = ex.Message;
              }
              return _result_;
        }
          public void SendMessageToUser(int ID_,string in_ch, string usr,string msg)
          {
              try
              {
                  MailMessage mail = new MailMessage();
                  SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
                  smtpServer.UseDefaultCredentials = false;
                  smtpServer.Credentials = new System.Net.NetworkCredential("sender@domain.com", "password");
                  smtpServer.EnableSsl = true;
                  smtpServer.Port = 587; // Gmail works on this ports
                  smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
                  mail.From = new MailAddress("sender@domian.com");
                  mail.To.Add("destination@domian.com");
                  mail.To.Add("destination@domian.com");
                  mail.To.Add(usr + "@domain.com");
                  mail.IsBodyHtml = true;
                  mail.Subject = in_ch + " - " + ID_ + " - Ticket Fechado";
                  var Body_ = "Exmo(a) Srº(ª),<br/>" +
                    "O sistema HelpDesk informa que o seu ticket com o ID " + ID_ + " FOI FECHADO pela aplicação.<br/>" +
                    "<br/>" +
                    "<br/>" +
                    "Razão: " + msg + "<br/>" +
                    "<br/>" +
                    "<br/>" +
                     "Mensagem enviada automaticamente - P.f. não responda a este e-mail!<br/>"+
                     "Caso não se confirme a resolução, solicita-se a reabertura do ticket!";
                  mail.Body = Body_;
                  smtpServer.Send(mail);
              }
              catch (Exception ex)
              {
                  _db_result = ex.Message;
              }
          }
    }
}
