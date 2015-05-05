using System;
using WCTPHost.App_Start;

namespace WCTPHost
{
    public class WebApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            WCTPlib.WCTPConfig.RegisterRoutes(WCTPConfig.Register);
        }
    }
}
