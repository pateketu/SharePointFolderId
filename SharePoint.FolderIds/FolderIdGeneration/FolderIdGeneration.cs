using System;
using System.Security.Permissions;
using Microsoft.SharePoint;
using Microsoft.SharePoint.Administration;
using Microsoft.SharePoint.Utilities;
using Microsoft.SharePoint.Workflow;

namespace SharePoint.FolderIds.FolderIdGeneration
{
    /// <summary>
    /// List Item Events
    /// </summary>
    public class FolderIdGeneration : SPItemEventReceiver
    {
        /// <summary>
        /// An item was added.
        /// </summary>
        public override void ItemAdded(SPItemEventProperties properties)
        {
            this.EventFiringEnabled = false;
            try
            {
                if (FolderIdService.IsFolder(properties.ListItem))
                {
                    using (SPWeb web = properties.OpenWeb())
                    {
                        FolderIdService.AssignId(web, properties.ListItem);
                    }

                }
            }
            catch (Exception exception)
            {
                SPDiagnosticsService diagSvc = SPDiagnosticsService.Local;
                diagSvc.WriteTrace(0,
                    new SPDiagnosticsCategory("Folder Id Service", TraceSeverity.Unexpected, EventSeverity.ErrorCritical),
                    TraceSeverity.Unexpected, "Message: {0}, StackTrace: {1}", exception.Message, exception.StackTrace);
            }
            finally
            {
                this.EventFiringEnabled = true;
            }
            
        }


    }
}