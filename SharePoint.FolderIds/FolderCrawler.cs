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
    public class FolderCrawler:TimerJobUtility
    {
        private bool shouldCancel;
        public FolderCrawler(string name, SPJobState state) : base(name,state)
        {
            shouldCancel = false;
        }

        
        private bool OnWebCrawlError(SPWeb web, Exception e)
        {
            //Log
            return false;
        }

        private void OnWebCrawl(SPWeb web)
        {
            this.ProcessLists(web.Lists,OnListCrawl, OnListCrawlError);
        }

        private bool OnListCrawlError(SPList list, Exception e)
        {
            return false;
        }

        private void OnListCrawl(SPList list)
        {
            if (list.BaseType == SPBaseType.DocumentLibrary)
            {
                SPQuery query = new SPQuery
                {
                    ViewAttributes = "Scope=\"Recursive\"",
                    Query = ItemsOfContentTypeOrChildQuery(SPBuiltInContentTypeId.Folder.ToString())
                };
                try
                {
                    this.ProcessListItems(list, query, OnItemCrawl, OnItemError);
                }
                catch (Exception exception)
                {
                    shouldCancel = true;
                }
                
            }
        }

        private bool OnItemError(SPListItem item, Exception e)
        {
            //Log
            return false;
        }

        private void OnItemCrawl(SPListItem item)
        {
            //Just Force Assign Folder Ids
            FolderIdService.AssignId(item.Web,item);
        }

        public void ProcessSingleWorkItem(SPWorkItem wi, WorkItemTimerJobState timerjobstate)
        {
            this.ProcessSite(timerjobstate.Site, OnWebCrawl, OnWebCrawlError);
        }

        protected override bool ShouldCancelCore(IterationGranularity granularity)
        {
            return base.ShouldCancelCore(granularity) || shouldCancel;
            
        }
    }
}
