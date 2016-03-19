using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LibPoco
{

    [Dapper.Contrib.Extensions.Table("Errors")]
    public class Errors
    {

        public Errors() { }
        public Errors(DateTime timeCreated, string message, string stackTrace, string recursiveStackTrace) 
        {
            this.TimeCreated = timeCreated;
            this.Message = message;
            this.StackTrace = stackTrace;
            this.RecursiveStackTrace = recursiveStackTrace;
        }

        [Dapper.Contrib.Extensions.Key()]
        public int Id { get; set; }

        public DateTime TimeCreated { get; set; }

        public string Message { get; set; }

        public string StackTrace { get; set; }

        public string RecursiveStackTrace { get; set; }

    }
}