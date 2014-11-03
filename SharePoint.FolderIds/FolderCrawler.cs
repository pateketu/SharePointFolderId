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
        private bool _shouldCancel;
        public FolderCrawler(string name, SPJobState state) : base(name,state)
        {
            _shouldCancel = false;
            //this.StrictQuerySemantics = false;
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
            if (list.BaseTemplate == SPListTemplateType.DocumentLibrary) //only looking into Document Libs
            {
                EnsureContentTypeIndexed(list); //ContentTypeId must be indexed
                
                SPQuery query = new SPQuery
                {
                    ViewAttributes = "Scope=\"RecursiveAll\"",
                    Query = ItemsOfContentTypeOrChildQuery(SPBuiltInContentTypeId.Folder.ToString())
                };
                try
                {
                    
                    this.ProcessListItems(list,query,OnItemsCrawl, OnItemsError);
                }
                catch (Exception exception)
                {
                    _shouldCancel = true;
                }
                
            }
        }

        private bool OnItemsError(SPListItemCollection items, Exception e)
        {
            //Log
            return false;
        }


     
        private void OnItemsCrawl(SPListItemCollection items)
        {

            int index = 0;
            while (index < items.Count)
            {
                FolderIdService.AssignId(items.List.ParentWeb, items[index]);
              
                if (this.ShouldCancel(IterationGranularity.Item))
                    break;
                checked { ++index; }
            }
           
        }

        public void ProcessSingleWorkItem(SPWorkItem wi, WorkItemTimerJobState timerjobstate)
        {
            this.ProcessSite(timerjobstate.Site, OnWebCrawl, OnWebCrawlError);
        }

        protected override bool ShouldCancelCore(IterationGranularity granularity)
        {
            return base.ShouldCancelCore(granularity) || _shouldCancel;
            
        }
    }
}
