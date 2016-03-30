using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TapeRestApi.Config;
using TapeRestApi.Models;

namespace TapeRestApi.Helpers
{
    public static class JobHelper
    {
        //private static readonly string FilePath = Path.Combine(GlobalSetting.UnManagedFolderPath, "TapeJob.xml");
        private static int _jobId = 0;
        private static readonly ConcurrentDictionary<int, JobInfo> Jobs = new ConcurrentDictionary<int, JobInfo>();
        private static Timer workTimer = new Timer(state =>
                                                  {
                                                      List<int> removeJobIds = new List<int>();

                                                      foreach (var item in Jobs)
                                                      {
                                                          double minuteSpan = (DateTime.Now - item.Value.BeginDateTime).TotalMinutes;

                                                          if (item.Value.State == JobState.Completed)
                                                          {
                                                              if (minuteSpan >= 5)
                                                              {
                                                                  removeJobIds.Add(item.Key);
                                                              }
                                                          }
                                                          else
                                                          {
                                                              if (minuteSpan < 2)
                                                              {
                                                                  continue;
                                                              }

                                                              item.Value.State = JobState.Completed;
                                                              item.Value.DateCompleted = DateTime.Now;
                                                              item.Value.ExitCode = 0;
                                                          }
                                                      }

                                                      JobInfo jobInfo;
                                                      foreach (int jobId in removeJobIds)
                                                      {                                                          
                                                          Jobs.TryRemove(jobId, out jobInfo);
                                                      }

                                                  }, null, TimeSpan.FromMinutes(1), TimeSpan.FromSeconds(30));

        public static JobResponse AddJob(string managedFilePath, string unmanagedCacheFilePath)
        {
            int jobId = Interlocked.Increment(ref _jobId);

            Jobs.TryAdd(jobId, new JobInfo
            {
                JobId = jobId,
                State = JobState.Running,
                BeginDateTime = DateTime.Now,
                SourceFilePath = managedFilePath
            });

            JobResponse jobResponse = new JobResponse
            {
                JobId = jobId
            };

            return jobResponse;
        }

        public static JobInfo GetJobInfo(int jobId)
        {
            return Jobs[jobId];
        }
    }
}
