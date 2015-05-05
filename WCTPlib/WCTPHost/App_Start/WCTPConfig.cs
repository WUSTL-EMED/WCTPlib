using System;
using WCTPHost.Controllers;
using WCTPlib;
using WCTPlib.v1r1;

namespace WCTPHost.App_Start
{
    public static class WCTPConfig
    {
        public static void Register(RouteTable table)
        {
            table.MapRoute(typeof(StatusInfo.Notification), typeof(OperationsController), "StatusInfoNotification");
        }
    }
}