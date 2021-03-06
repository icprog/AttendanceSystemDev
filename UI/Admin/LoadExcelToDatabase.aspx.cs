﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using BLL;

public partial class Admin_LoadExcelToDatabase : System.Web.UI.Page
{
    string currFilePath = string.Empty;  //要导入的excel文件路径--服务器端
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (!IsPostBack)
        //{
        //    if (Session["uesrID"].ToString() == "")
        //    {
        //        Response.Redirect("..\\Login.aspx");
        //    }
        //    else
        //    {
        //        btnClearPreData.Attributes.Add("onclick", "return confirm('本操作将清空所有数据表,确定要执行这个操作吗?');");
        //        //btnPreOperation.Attributes.Add("onclick", "return confirm('本操作将颠覆原有数据,确定要执行这个操作吗?')");
        //        //btnTeacherAttendance.Attributes.Add("onclick", "return confirm('本操作覆盖原有数据，确定要执行这个操作吗?')");
        //    }
        //}
        Clear();
    }

    private bool UpLoad(FileUpload fileUpload)
    {
        string fileExt = string.Empty;  //文件扩展名

        if (fileUpload.HasFile)
        {
            fileExt = System.IO.Path.GetExtension(fileUpload.FileName);//获取文件后缀

            if (fileExt == ".xls" || fileExt == ".xlsx")
            {
                try
                {
                    this.currFilePath = Server.MapPath("../") + "TempFile\\" + DateTime.Now.ToString("yyyyMMddhhmmss") + fileExt;//服务器路径
                    fileUpload.SaveAs(this.currFilePath);//上传
                    return true;
                }
                catch (Exception ex)
                {
                    Response.Write("<script>alert('发生错误：'" + ex.Message.ToString() + ")</script>");
                }
            }
            else
            {
                Response.Write("<script>alert('只允许导入xls、xlsx文件！')</script>");
            }
        }
        else
        {
            Response.Write("<script>alert('没有选择要导入的文件！')</script>");
        }
        return false;
    }

    protected void BtnImportTeachers_Click(object sender, EventArgs e)
    {
        //  Clear();
        string identity = "";
        if (rdoTeacher.Checked | rdoOther.Checked)
        {
            if (rdoTeacher.Checked)
                identity = "TabTeachers";
            else
                identity = "TabOtherTeachers";
            if (UpLoad(FileUploadTeacher))
                lblMessage1.Text = ExcelToDatabase.CheckFile(currFilePath, identity);
        }
        else
        {
            Response.Write("<script>alert('请选择导入的数据是本地教师或外聘教师！')</script>");
        }
    }

    protected void BtnImportCourse_Click(object sender, EventArgs e)
    {
        Clear();
        string department = "";
        department = ddlDepartmentName.SelectedItem.ToString();
        if (UpLoad(FileUploadCourse))
            lblMessage2.Text = ExcelToDatabase.CheckFile(currFilePath, department);
    }

    private void Clear()
    {
        lblMessage1.Text = "";
        lblMessage2.Text = "";
        lblMessage5.Text = "";
        lblMessage7.Text = "";
    }

    //导入校历
    protected void BtnImportCalendar_Click(object sender, EventArgs e)
    {
        Clear();
        // AddSQLStringToDAL.DelectTabTeachers("TabCalendar");
        if (UpLoad(FileUploadCalendar))
            lblMessage5.Text = ExcelToDatabase.CheckFile(currFilePath, "TabCalendar");
    }

    protected void BtnDepartmentCount_Click(object sender, EventArgs e)
    {
        string[] str = { "会计系", "信息工程系", "经济管理系", "食品工程系", "机械工程系", "商务外语系", "建筑工程系" };
        int[] sum = new int[str.Length];
        if (txtKJ.Text != "" && txtXX.Text != "" && txtJG.Text != "" && txtSP.Text != "")
        // && txtJX.Text != "" && txtWY.Text != "" && txtJZ.Text != ""
        {
            sum[0] = Convert.ToInt32(txtKJ.Text.Trim());
            sum[1] = Convert.ToInt32(txtXX.Text.Trim());
            sum[2] = Convert.ToInt32(txtJG.Text.Trim());
            sum[3] = Convert.ToInt32(txtSP.Text.Trim());
            //sum[4] = Convert.ToInt32(txtJX.Text.Trim());
            //sum[5] = Convert.ToInt32(txtWY.Text.Trim());
            //sum[6] = Convert.ToInt32(txtJZ.Text.Trim());
        }
        if (AddSQLStringToDAL.DeleteTabTeachers("TabDepartment"))
        {
            for (int i = 0; i < str.Length; i++)
            {
                string strSql = "INSERT INTO TabDepartment VALUES('" + str[i] + "','" + sum[i] + "')";
                if (AddSQLStringToDAL.InsertData(strSql))
                {
                    Label16.Text = "各系人数设置完毕";
                }
                else
                {
                    Label16.Text = "设置失败";
                }
            }
        }

    }

    protected void btnClear_Click(object sender, EventArgs e)
    {
        if (true)
        {
            
            if (ExcelToDatabase.ClearExcel())
            {
                lblMessage7.Text = "清除完成";
            }
            else
            {
                lblMessage7.Text = "清除失败";
            }
        }
    }
    /// <summary>
    /// 将入库数据按需提取，并拆分
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Button5_Click(object sender, EventArgs e)
    {
        //拆分课程信息，保存到TabTeacherAllCourse
        lblMessage7.Text = DatabaseToDatabase.TabTeacherAllCourse();


    }
    protected void btnPrOperation_Click(object sender,EventArgs e)
    {
        DataTable dt = AddSQLStringToDAL.GetDatatableBySQL("TabTeachers");
        if(dt.Rows.Count>0)
        {
            if (dt.Rows[0]["UserID"].ToString()==dt.Rows[0]["UserPWD"].ToString())
            {
                initalPWD();
            }
        }
    }

    private void initalPWD()
    {
        List<string> str = new List<string>();
        str = AddSQLStringToDAL.GetDistinctString("TabTeachers", "UserID");
        for(int i=0;i<str.Count;i++)
        {
            if (AddSQLStringToDAL.UpdataTabTeachers("TabTeachers", PWDProcess.MD5Encrypt(str[i].ToString(), PWDProcess.CreatKey(str[i].ToString())), str[i].ToString())) ;

            {

            }
        }
        List<string> str2 = new List<string>();
        str2 = AddSQLStringToDAL.GetDistinctString("TabOtherTeachers", "UserID");
        for (int i=0;i<str2.Count;i++) {
            if (AddSQLStringToDAL.UpdataTabTeachers("TabOtherTeachers", PWDProcess.MD5Encrypt(str2[i].ToString(), PWDProcess.CreatKey(str2[i].ToString())), str2[i].ToString())) 

            {
               
            }
        }
    }
}
