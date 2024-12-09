using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Configuration;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Security.Claims;

namespace DataTier
{
    // *********************************************************************************************
    //                                      Tools Class.
    // *********************************************************************************************
    public class Tools
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        // ---------------------------------------------------------------------------------------------
        //                  Constructor.
        // ---------------------------------------------------------------------------------------------
        public Tools(IHttpContextAccessor httpContextAccessor,
                            IWebHostEnvironment hostingEnvironment,
                            IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        // ---------------------------------------------------------------------------------------------
        //                  System event is generated.
        // ---------------------------------------------------------------------------------------------
        public void AddSystemEvent(string exceptionMessage, string exceptionType, string exceptionStack, string exceptionDetails = "")
        {
            // Name of file where event will be saved.
            // The name of file is composed of current date (yyyy-mm-dd) with extension ".log".
            string eventsFile = "";
            eventsFile += DateTime.Now.Year.ToString() +
                '-' + DateTime.Now.Month.ToString().PadLeft(2, '0') +
                '-' + DateTime.Now.Day.ToString().PadLeft(2, '0') +
                ".log";

            // System events location directory.
            string eventsDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "SystemEvents", eventsFile);

            // System event is generated.
            StreamWriter eventsStreamFile = new StreamWriter(eventsDirectory, true);
            eventsStreamFile.WriteLine("---> Date : " + DateTime.Now.ToString("yyyy/MM/dd HH24:MI:SS AD") + "\t");
            eventsStreamFile.WriteLine("     Specs: " + exceptionDetails + "\t");
            eventsStreamFile.WriteLine("     Msg  : " + exceptionMessage + "\t");
            eventsStreamFile.WriteLine("     Type : " + exceptionType + "\t");
            eventsStreamFile.WriteLine("     Stack: " + exceptionStack);
            eventsStreamFile.WriteLine("");
            eventsStreamFile.Close();
        }

        // ---------------------------------------------------------------------------------------------
        //       Get connection string content based on first role the current user belongs to.
        // ---------------------------------------------------------------------------------------------
        public string GetConnectionStringContent(string connectionStringName)
        {
            return _configuration.GetConnectionString(connectionStringName);
        }

        // ---------------------------------------------------------------------------------------------
        //          Get the name of the user that is currently logged in
        // ---------------------------------------------------------------------------------------------
        public string GetLoggedUserName()
        {
            string userName = _httpContextAccessor.HttpContext.User.Identity.Name;

            if (string.IsNullOrEmpty(userName))
                userName = "Anonymous";

            return userName;
        }

        public string GetUserRole()
        {
            var currentUserRoleId = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            
            return currentUserRoleId;
        }
    }
}