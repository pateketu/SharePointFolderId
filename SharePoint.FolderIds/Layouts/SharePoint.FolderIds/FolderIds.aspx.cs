using System;
using Microsoft.SharePoint;
using Microsoft.SharePoint.WebControls;

namespace SharePoint.FolderIds.Layouts.SharePoint.FolderIds
{
    public partial class FolderIds : LayoutsPageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Reset(object sender, EventArgs e)
        {
            SPContext.Current.Site.AddWorkItem(Guid.Empty, DateTime.Now.ToUniversalTime(),
                FolderIdWorkItemJobDefinition.FolderIdWorkItemType, SPContext.Current.Site.RootWeb.ID, SPContext.Current.Site.ID, 1, false, Guid.Empty,
                Guid.Empty, SPContext.Current.Web.CurrentUser.ID, null, null, Guid.Empty);
        }
    }
}
