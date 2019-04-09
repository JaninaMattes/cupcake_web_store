﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebsiteLaitBrasseur.Admin
{
    public partial class LogoutAdmin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            HttpCookie email = Request.Cookies["email"];
            if (email != null)
            {
                lblGoodBye.Text = "Good bye " + email.Value;
            }
        }
    }
}