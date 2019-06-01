﻿using Microsoft.AspNet.FriendlyUrls;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebsiteLaitBrasseur.UL.Admin
{
    public partial class Default : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.Session["AdminID"] == null)
            {
                var urlF = FriendlyUrl.Href(ConfigurationManager.AppSettings["Admin"] + "LoginAdmin");

                Response.Redirect(ConfigurationManager.AppSettings["SecurePath"] + urlF);
            }
        }

        protected void Shippingbutton_Click(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings["SecurePath"] + ConfigurationManager.AppSettings["Admin"] + "PostagesManagement.aspx";
            var urlF = FriendlyUrl.Href("~/PostagesManagment.aspx");
            Response.Redirect(url);

        }

        protected void Customerbutton_Click(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings["SecurePath"] + ConfigurationManager.AppSettings["Admin"] + "AccountManagment.aspx";
            Response.Redirect(url);

        }

        protected void Productbutton_Click(object sender, EventArgs e)
        {
            string url = ConfigurationManager.AppSettings["SecurePath"] + ConfigurationManager.AppSettings["Admin"] + "ItemsManagement.aspx";
            Response.Redirect(url);
        }
    }
}