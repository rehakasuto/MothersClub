using MothersClub.Models;
using System;
using System.Diagnostics;
using System.Reflection;

namespace MothersClub.Service
{
    public class ActivityService
    {
        public static void Log(string message, string requestIpAddress = null, int? userId = null, string userName = null)
        {
            try
            {
                using (MCContext ctx = new MCContext())
                {
                    ctx.systemLogs.Add(new SystemLog()
                    {
                        logMessage = message,
                        requestIpAddress = requestIpAddress,
                        userId = userId,
                        userName = userName
                    });
                    ctx.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                LogException(ex);
            }
        }


        public static void LogException(Exception exception)
        {
            try
            {
                if (exception != null)
                {
                    StackFrame frame = new StackFrame(1);
                    MethodBase methodBase = frame.GetMethod();

                    string methodName = methodBase.Name;
                    string className = methodBase.DeclaringType.Name;

                    string msg = "";
                    int exCount = 1;
                    Exception ex = exception;
                    while (ex != null)
                    {
                        msg += exCount + "\n" + ex.Message + "\n" + ex.StackTrace + "\n\n";
                        ex = ex.InnerException;
                        exCount++;
                    }

                    using (MCContext context = new MCContext())
                    {
                        ExceptionLog log = new ExceptionLog()
                        {
                            function = methodName,
                            objectClass = className,
                            exceptionMessage = msg,
                        };
                        context.exceptionLogs.Add(log);
                        context.SaveChanges();
                    }
                }
            }
            catch (Exception ex) { }
        }
    }

}