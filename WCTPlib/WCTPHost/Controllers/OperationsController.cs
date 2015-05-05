using System;
using System.Diagnostics;
using WCTPlib;
using WCTPlib.v1r1;

namespace WCTPHost.Controllers
{
    public class OperationsController : OperationController
    {
        //use overloads instead of differently named methods?
        public WCTPlib.Operation StatusInfoNotification(StatusInfo.Notification notification)
        {
            //Do stuff
            return new Confirmation.Success((int)Status.Acknowledged) { SuccessText = "Acknowledged" };
        }
    }
}