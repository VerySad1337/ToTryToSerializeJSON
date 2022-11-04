using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using Newtonsoft.Json;
using System.Collections;

namespace ResearchConference

{
    public partial class ViewReview : System.Web.UI.Page
    {
        SqlConnection dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["RCMSConnectionString"].ConnectionString);
        class viewReviewController
        {
            public string PaperTitle { get; set; }
            public string PaperURL { get; set; }
            int MyUserID; 

            public SqlDataAdapter viewReviewTable(string getUserID)
            {
                SqlConnection dbConnections = new SqlConnection(ConfigurationManager.ConnectionStrings["RCMSConnectionString"].ConnectionString);
                dbConnections.Open();
                string currentSessionID = getUserID;
                viewReviewEntity myEntity = new viewReviewEntity(); //Dont Make any sense if u mark, but you require BCE!.
                string iDontUnderstandWhy = myEntity.getUserID(currentSessionID); //Dont make sense with BCE, slowing down the operation!
                SqlDataAdapter SQLQuery = new SqlDataAdapter("Select PaperTitle,URL from paper where useridposted = " + iDontUnderstandWhy, dbConnections);
                dbConnections.Close();
                return SQLQuery;
            }

            ArrayList ALPaperTitle = new ArrayList();
            public ArrayList tryOutNewLogic(int currentUserSessionID)
            {
                SqlConnection dbConnections = new SqlConnection(ConfigurationManager.ConnectionStrings["RCMSConnectionString"].ConnectionString);
                dbConnections.Open();
                MyUserID = currentUserSessionID;
                ArrayList returnedPaperTitle = new ArrayList();
                string getPaperTitleSQL = "Select PaperTitle from Paper where useridposted=" + MyUserID;
                SqlCommand command = new SqlCommand(getPaperTitleSQL, dbConnections);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ALPaperTitle.Add((string)reader["PaperTitle"]);
                    }
                }
                dbConnections.Close();
                return ALPaperTitle;
            }

            ArrayList ALPaperURL = new ArrayList();
            public ArrayList tryOutNewLogic1(int currentUserSessionID)
            {
                SqlConnection dbConnections = new SqlConnection(ConfigurationManager.ConnectionStrings["RCMSConnectionString"].ConnectionString);
                dbConnections.Open();
                MyUserID = currentUserSessionID;
                ArrayList returnedPaperTitle = new ArrayList();
                string getPaperURLSQL = "Select URL from Paper where useridposted=" + MyUserID;
                SqlCommand command = new SqlCommand(getPaperURLSQL, dbConnections);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ALPaperURL.Add((string)reader["URL"]);
                    }
                }
                dbConnections.Close();
                return ALPaperURL;
            }

        }

        class viewReviewEntity
        {
            public string getUserID(string fromControllerUserID)
            {
                string currentSessionID = fromControllerUserID;
                return currentSessionID;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserID"] == null)
            {
                Response.Redirect("ReviewerLogin.aspx");
            }
            else
            {
                if (int.Parse(Session["roleid"].ToString()) == 3)
                {
                    dbConnection.Open();
                    string myUserID = Session["UserID"].ToString();
                    viewReviewController createTable = new viewReviewController();
                    DataTable myDataTable = new DataTable();
                    using (dbConnection)
                    {
                        createTable.viewReviewTable(myUserID).Fill(myDataTable);
                        GridView2.DataSource = myDataTable;
                        GridView2.DataBind();
                        dbConnection.Close();
                    }

                    JsonSerializer serializer = new JsonSerializer();
                    viewReviewController entity = new viewReviewController();
                    int SessionUserID = int.Parse(Session["UserID"].ToString());
                    var myArrayList = entity.tryOutNewLogic(SessionUserID);
                    var myArrayList1 = entity.tryOutNewLogic1(SessionUserID);

                    var myJSON = new ArrayList();
                    for (int i = 0; i < myArrayList.Count; i++)
                    {
                        string myOutput = myArrayList[i].ToString();
                        string myOutput1 = myArrayList1[i].ToString();
                        var JSONArray = new viewReviewController()
                        {
                            PaperTitle = myOutput,
                            PaperURL = myOutput1
                        };
                        myJSON.Add(JSONArray);
                    }
                    var PostJSON = JsonConvert.SerializeObject(myJSON);
                    Response.Write(PostJSON);
                }
                else
                {
                    Label3.Text = "Why are you here? You are not reviewer!";
                }
            }

        }

        protected void DataList1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void ViewReviewFromDB_Selecting(object sender, SqlDataSourceSelectingEventArgs e)
        {

        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void GridView1_RowEditing(object sender, GridViewEditEventArgs e)
        {

        }

        protected void GridView1_SelectedIndexChanged1(object sender, EventArgs e)
        {

        }

        protected void giveReview_Click(object sender, EventArgs e)
        {
            int PaperID = Convert.ToInt32((sender as LinkButton).CommandArgument);
            string PaperIDSession = PaperID.ToString();
            Session["PaperIDFromRow"] = PaperIDSession;
            Response.Redirect("AddReview.aspx");
        }
        protected void giveComments_Click(object sender, EventArgs e)
        {
            int PaperID = Convert.ToInt32((sender as LinkButton).CommandArgument);
            string PaperIDSession = PaperID.ToString();
            Session["PaperIDFromRow"] = PaperIDSession;
            Response.Redirect("AddComments.aspx");
        }
    }
}