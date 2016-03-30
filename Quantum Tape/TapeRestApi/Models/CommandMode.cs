using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace TapeRestApi.Models
{
    public enum CommandMode
    {
        Async,
        Sync
    }

    public enum JobState
    {
        Pending,
        Error,
        Running,
        Completed
    }
}
