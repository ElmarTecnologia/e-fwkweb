using log4net;
using System;
using System.Web;

namespace ELMAR.DevHtmlHelper.Models
{
    public class Log
    {
        private static readonly Log _instance = new Log();
        protected ILog monitoringLogger;
        protected static ILog debugLogger;

        private Log()
        {
            monitoringLogger = LogManager.GetLogger("MonitoringLogger");
            debugLogger = LogManager.GetLogger("DebugLogger");            
        }

        private static void SetLogContext(HttpContext httpContext)
        {
            string CTX = string.Empty;
            if (httpContext == null)
            {
                log4net.GlobalContext.Properties["LogFileName"] = "Log_" + DateTime.Now.ToString("ddMMyyyy") + ".log";
            }
            else
            {
                HttpSessionStateBase Session = new HttpContextWrapper(httpContext).Session;
                CTX = Core.GetSetCTX(new HttpContextWrapper(httpContext));
                if (!CTX.Equals(string.Empty))
                {
                    log4net.GlobalContext.Properties["LogFileName"] = "Log_" + CTX + "_" + DateTime.Now.ToString("ddMMyyyy") + ".log";
                }
                if (!CTX.Equals(string.Empty) && Session["USER"] != null)
                {
                    log4net.GlobalContext.Properties["LogFileName"] = "Log_" + CTX + "_" + Session["USER_ID"].ToString() + "_" + DateTime.Now.ToString("ddMMyyyy") + ".log";
                }
            }                        
            ChangeFilePath("RollingFile", log4net.GlobalContext.Properties["LogFileName"].ToString());
        }

        /// <summary>  
        /// Used to log Debug messages in an explicit Debug Logger  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        public static void Debug(string message, HttpContext httpContext = null)
        {
            SetLogContext(httpContext);
            debugLogger.Debug(message);
        }


        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        /// <param name="exception">The exception to log, including its stack trace </param>  
        public static void Debug(string message, System.Exception exception, HttpContext httpContext = null)
        {
            SetLogContext(httpContext);
            debugLogger.Debug(message, exception);
        }


        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        public static void Info(string message, HttpContext httpContext = null)
        {
            SetLogContext(httpContext);
            _instance.monitoringLogger.Info(message); 
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        /// <param name="exception">The exception to log, including its stack trace </param>  
        public static void Info(string message, System.Exception exception, HttpContext httpContext = null)
        {
            SetLogContext(httpContext);
            _instance.monitoringLogger.Info(message, exception);
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        public static void Warn(string message, HttpContext httpContext = null)
        {
            SetLogContext(httpContext);
            _instance.monitoringLogger.Warn(message);
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        /// <param name="exception">The exception to log, including its stack trace </param>  
        public static void Warn(string message, System.Exception exception, HttpContext httpContext = null)
        {
            SetLogContext(httpContext);
            _instance.monitoringLogger.Warn(message, exception);
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        public static void Error(string message, HttpContext httpContext = null)
        {
            SetLogContext(httpContext);
            _instance.monitoringLogger.Error(message);
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        /// <param name="exception">The exception to log, including its stack trace </param>  
        public static void Error(string message, System.Exception exception, HttpContext httpContext = null)
        {
            SetLogContext(httpContext);
            _instance.monitoringLogger.Error(message, exception);
        }


        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        public static void Fatal(string message, HttpContext httpContext = null)
        {
            SetLogContext(httpContext);
            _instance.monitoringLogger.Fatal(message);
        }

        /// <summary>  
        ///  
        /// </summary>  
        /// <param name="message">The object message to log</param>  
        /// <param name="exception">The exception to log, including its stack trace </param>  
        public static void Fatal(string message, System.Exception exception, HttpContext httpContext = null)
        {
            SetLogContext(httpContext);
            _instance.monitoringLogger.Fatal(message, exception);
        }

        public static void ChangeFilePath(string appenderName, string newFilename)
        {
            log4net.Repository.ILoggerRepository repository = log4net.LogManager.GetRepository();
            foreach (log4net.Appender.IAppender appender in repository.GetAppenders())
            {
                if (appender.Name.CompareTo(appenderName) == 0 && appender is log4net.Appender.FileAppender)
                {
                    log4net.Appender.FileAppender fileAppender = (log4net.Appender.FileAppender)appender;
                    fileAppender.File = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(fileAppender.File), newFilename);
                    fileAppender.ActivateOptions();
                }
            }
        }
    }  
}