using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Server.Utilities;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;

namespace SharePoint.FolderIds
{
    public class FolderIdWorkItemJobDefinition : SPWorkItemJobDefinition
    {
        private WorkItemTimerJobState _timerJobState;

        public override string DisplayName
        {
            get
            {
                return "Folder Id Assignment";
            }
        }

        public override string Description
        {
            get
            {
                return "Assigns Folder Ids to the Folders in Site Collections on which Document ID Service & Folder ID Feature is enabled";
            }
        }

        public override bool EnableBackup
        {
            get
            {
                return true;
            }
        }

        public static Guid FolderIdWorkItemType
        {
            get
            {
                return new Guid("75B7A4D9-4B48-434E-B389-1E7183A4FB93");
            }
        }

        public static string FolderIdWorkItemJobName
        {
            get
            {
                return "FolderIdAssignment";
            }
        }
        public FolderIdWorkItemJobDefinition()
        {
            
        }

        public FolderIdWorkItemJobDefinition(SPWebApplication webApplication)
            : base(FolderIdWorkItemJobName, webApplication)
        {
        }

        public override Guid WorkItemType()
        {
            return FolderIdWorkItemType;
        }
        public override void Execute(SPJobState jobState)
        {
            this._timerJobState = new WorkItemTimerJobState(true);
            try
            {
                base.Execute(jobState);
            }
            finally
            {
                this._timerJobState.Dispose();
                this._timerJobState = null;
            }
            
        }


        protected override bool ProcessWorkItem(SPContentDatabase contentDatabase, SPWorkItemCollection workItems, SPWorkItem workItem,
            SPJobState jobState)
        {

            FolderCrawler folderCrawler = new FolderCrawler(this.DisplayName, jobState);
            folderCrawler.CancellationGranularity = IterationGranularity.Item;
            folderCrawler.ResumeGranularity = IterationGranularity.List;
            folderCrawler.DisableEventFiring = true;
            return folderCrawler.ProcessWorkItem(workItems, workItem, this._timerJobState,
                folderCrawler.ProcessSingleWorkItem);

        }

        internal static void Register(SPWebApplication webApp)
        {
            if (webApp.JobDefinitions.GetValue<FolderIdWorkItemJobDefinition>(FolderIdWorkItemJobName) == null)
            {
                FolderIdWorkItemJobDefinition itemJobDefinition = new FolderIdWorkItemJobDefinition(webApp)
                {
                    Schedule = SPSchedule.FromString("daily between 23:00:00 and 00:30:00")
                };
                itemJobDefinition.Update();
            }
        }

        internal static void UnRegister(SPWebApplication webApp)
        {
            FolderIdWorkItemJobDefinition def =
                webApp.JobDefinitions.GetValue<FolderIdWorkItemJobDefinition>(FolderIdWorkItemJobName);
            if(def != null) def.Delete();
        }
    }
}
