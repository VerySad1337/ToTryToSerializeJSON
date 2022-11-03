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
using System.IO;
using System.Collections;

namespace ResearchConference
{
    public partial class ViewReview : System.Web.UI.Page
    {
        SqlConnection dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["RCMSConnectionString"].ConnectionString);

        class ViewReviewEntity
        {
            SqlConnection dbConnection2 = new SqlConnection(ConfigurationManager.ConnectionStrings["RCMSConnectionString"].ConnectionString);

            int myUserID;

            public string PaperTitle { get; set; }
            public string PaperURL { get; set; }

            ArrayList ALPaperTitle = new ArrayList();
            public ArrayList tryOutNewLogic(int currentUserSessionID)
            {
                dbConnection2.Open();
                myUserID = currentUserSessionID;
                ArrayList returnedPaperTitle = new ArrayList();
                string getPaperTitleSQL = "Select PaperTitle from Paper where useridposted=" + myUserID;
                SqlCommand command = new SqlCommand(getPaperTitleSQL, dbConnection2);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ALPaperTitle.Add((string)reader["PaperTitle"]);
                    }
                }
                return ALPaperTitle;
            }

            ArrayList ALPaperURL = new ArrayList();
            public ArrayList tryOutNewLogic1(int currentUserSessionID)
            {
                myUserID = currentUserSessionID;
                ArrayList returnedPaperTitle = new ArrayList();
                string getPaperURLSQL = "Select URL from Paper where useridposted=" + myUserID;
                SqlCommand command = new SqlCommand(getPaperURLSQL, dbConnection2);
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ALPaperURL.Add((string)reader["URL"]);
                    }
                }
                return ALPaperURL;
            }

            public string getPaperTitle(int currentUserSessionID)
            {
                myUserID = currentUserSessionID;
                string getPaperTitleSQL = "Select PaperTitle from Paper where useridposted=" + myUserID;
                SqlCommand displayResult = new SqlCommand(getPaperTitleSQL, dbConnection2);
                string retrievePaperTitle = displayResult.ExecuteScalar().ToString();
                return retrievePaperTitle;
            }

            public string getPaperURL(int currentUserSessionID)
            {
                myUserID = currentUserSessionID;
                string getPaperTitleSQL = "Select URL from Paper where useridposted=" + myUserID;
                SqlCommand displayResult = new SqlCommand(getPaperTitleSQL, dbConnection2);
                string retrievePaperURL = displayResult.ExecuteScalar().ToString();
                return retrievePaperURL;
            }



        }
        //SqlConnection dbConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["RCMSConnectionString"].ConnectionString);
        //string dbConnection = @"Data Source=DESKTOP-0R2NCQ5;Initial Catalog = RCMS; Integrated Security = True";
        protected void Page_Load(object sender, EventArgs e)
        {
            JsonSerializer serializer = new JsonSerializer();
            ViewReviewEntity entity = new ViewReviewEntity();
            int SessionUserID = int.Parse(Session["UserID"].ToString());
            var myArrayList = entity.tryOutNewLogic(SessionUserID);
            var myArrayList1 = entity.tryOutNewLogic1(SessionUserID);

            var myJSON = new ArrayList();
            for (int i =0; i < myArrayList.Count; i++)
            {
                    string myOutput = myArrayList[i].ToString();
                    string myOutput1 = myArrayList1[i].ToString();
                    var JSONArray = new ViewReviewEntity()
                    {
                        PaperTitle = myOutput,
                        PaperURL = myOutput1
                    };
                    myJSON.Add(JSONArray);
            }
            var PostJSON = JsonConvert.SerializeObject(myJSON);
            Response.Write(PostJSON);

            //string myPaperTitle = entity.tryOutNewLogic(SessionUserID.ToString());
            //string myPaperURL = entity.getPaperURL(SessionUserID);
            //string JSONString = myPaperTitle + myPaperURL;
            //var JSONArray = new ViewReviewEntity()
            //{
            //  PaperTitle = myPaperTitle,
            //PaperURL = myPaperURL

            //};


            //string filePath = @"C:\Users\Eric\Desktop\JSONOutput\JSON1.txt";
            //using (var sw = new StreamWriter(filePath))
            //using (JsonWriter writer = new JsonTextWriter(sw))
            //{
            //  serializer.Deserialize(writer, JSONString);
            //}


            if (Session["UserID"] == null)
            {
                Response.Redirect("ReviewerLogin.aspx");
            }
            else
            {

                using (dbConnection)
                {
                    string currentSessionUserID = Session["UserID"].ToString();
                    dbConnection.Open();
                    SqlDataAdapter sqlda = new SqlDataAdapter("SELECT Allocation.AllocationID, Allocation.PaperID,  Allocation.UserID,Users.Name, Allocation.GradeID, Paper.Date, Paper.PaperTitle, Paper.URL, Allocation.PaperID as session FROM (Allocation  INNER JOIN Paper ON Allocation.PaperID = Paper.PaperID) INNER JOIN USERS on Allocation.UserID = Users.UserID where Allocation.UserID = " + currentSessionUserID  , dbConnection);

                    DataTable dtbl = new DataTable();
                    sqlda.Fill(dtbl);
                    GridView2.DataSource = dtbl;
                    GridView2.DataBind();

                    if(dtbl.Rows.Count == 0)
                    {
                        Label3.Text = "No paper assigned to you";
                    }
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