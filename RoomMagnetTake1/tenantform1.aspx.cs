﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Configuration;

public partial class tenantform1 : System.Web.UI.Page
{
    // connection string at the class level so we can reference it later. this is the string in the web.config !!
    SqlConnection sc = new SqlConnection(ConfigurationManager.ConnectionStrings["cs"].ConnectionString);

    protected void Page_Load(object sender, EventArgs e)
    {

    }


    protected void btnCommitTenant_Click(object sender, EventArgs e)
    {
        if (checkPasswordsMatches())
        {

            Customer newCustomer = new Customer(getString(txtEmail));

            SqlCommand customerInsert = new SqlCommand("INSERT INTO [dbo].[Customer] (Email, Password) VALUES( @email, @password)", sc);

            customerInsert.Parameters.AddWithValue("@email", newCustomer.getEmail());
            customerInsert.Parameters.AddWithValue("@password", PasswordHash.HashPassword(txtPassword.Text));

            try
            {
                sc.Open();
                customerInsert.ExecuteNonQuery();
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "NoDatabaseAlertMessage", "alert('New Host inserted')", true);
            }

            catch
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "NoDatabaseAlertMessage", "alert('customer NOT Inserted')", true);
            }
            finally
            {
                sc.Close();
            }

            //create new Tenant object
            Tenant newTenant = new Tenant(getString(txtFirstName), getString(txtLastName), ddGender.Text, getString(txtEmail), getString(txtPhonenumber), convertToDateFormat(combineBirthday()),
                ddTenantType.Text);

            //new paramterized query to insert a tenant
            SqlCommand command = new SqlCommand("INSERT into [dbo].[Tenant] (FirstName, LastName, Gender, Email, PhoneNumber, BirthDate, TenantType) " +
                "VALUES(@firstName, @lastName, @gender, @email, @phoneNumber, @dateOfBirth, @tenantType)", sc);

            command.Parameters.AddWithValue("@firstName", newTenant.getFirstName());
            command.Parameters.AddWithValue("@lastName", newTenant.getLastName());
            command.Parameters.AddWithValue("@gender", newTenant.getGender());
            command.Parameters.AddWithValue("@email", newTenant.getEmail());
            command.Parameters.AddWithValue("@phoneNumber", newTenant.getPhoneNumber());
            command.Parameters.AddWithValue("@dateOfBirth", newTenant.getDateOfBirth());
            command.Parameters.AddWithValue("@tenantType", newTenant.getTenantType());

            try
            {
                sc.Open();

                //sends query to SQL
                command.ExecuteNonQuery();

                //window to show success of insert
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "NoDatabaseAlertMessage", "alert('New Tenant inserted')", true);
                Response.Redirect("tenantform2.aspx");
            }
            catch
            {
                //throw error pop up if unsuccesful
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "NoDatabaseAlertMessage", "alert('Tenant NOT Inserted')", true);
            }

            finally
            {
                sc.Close();
            }

        }
    }

    public String getString(TextBox txt)
    {
        String returnString = HttpUtility.HtmlEncode(txt.Text);
        return returnString;
    }

    protected DateTime convertToDateFormat(String date)
    {
        DateTime parseValue = DateTime.Parse(date);
        return parseValue;
    }

    protected String combineBirthday()
    {
        String str = (ddMonth.Text + " " + ddDay.Text + ", " + ddYear.Text);
        return str;
    }

    protected Boolean checkPasswordsMatches()
    {
        Boolean retBool = false;

        if (txtPassword.Text.Equals(txtConfirmPassword.Text))
        {
            retBool = true;
        }

        else
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "Password Error", "alert('Passwords do not match')", true);
        }

        return retBool;
    }

}