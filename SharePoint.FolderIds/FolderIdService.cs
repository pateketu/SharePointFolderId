using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.DocumentManagement;
using Microsoft.Office.DocumentManagement.Internal;
using Microsoft.SharePoint;

namespace SharePoint.FolderIds
{
    public class FolderIdService
    {
        private const string DocIdFieldInternalName = "_dlc_DocId";
        private const string DocIdUrlFieldInternalName = "_dlc_DocIdUrl";

        public static bool IsFolder(SPListItem item)
        {
            return item[SPBuiltInFieldId.FSObjType] == null
               ? item.ContentTypeId.Equals(SPBuiltInContentTypeId.Folder) ||
                 item.ContentTypeId.IsChildOf(SPBuiltInContentTypeId.Folder)
               : item[SPBuiltInFieldId.FSObjType].ToString()
                   .Equals("1", StringComparison.InvariantCultureIgnoreCase);
        }

        public static bool HasId(SPListItem folderItem)
        {
            return folderItem[DocIdFieldInternalName] != null;
        }
        public static void AssignId(SPWeb web, SPListItem folderItem)
        {
            DocumentIdProvider provider = GetProvider(web.AllProperties);
            string id = provider.GenerateDocumentId(folderItem);
            folderItem[DocIdFieldInternalName] = id;
            folderItem[DocIdUrlFieldInternalName] = new SPFieldUrlValue()
            {
                Url = string.Format("{0}/_layouts/15/DocIdRedir.aspx?ID={1}", web.Url, id),
                Description = id
            };
            folderItem.SystemUpdate();
        }

        private static DocumentIdProvider GetProvider(Hashtable rootWebProperties)
        {
            //OOB Stores Assembly and Type Info of any custom Provider in SPWeb.AlllProperties with following 
            //two keys
            const string assemblyKey = "docid_customProvider_assembly"; 
            const string classKey = "docid_customProvider_class";
            DocumentIdProvider provider;
            if (rootWebProperties.ContainsKey(assemblyKey) && rootWebProperties.ContainsKey(classKey))
            {
                string assembly = rootWebProperties[assemblyKey].ToString();
                string name = rootWebProperties[classKey].ToString();
                    
                try
                {
                    provider = (DocumentIdProvider)Activator.CreateInstance(Assembly.Load(assembly).GetType(name, true, false));
                }
                catch (Exception ex)
                {
                    if (!(ex is NullReferenceException) && !(ex is SEHException))
                        throw new InvalidDocumentIdProviderException(assembly + ", " + name);
                    throw;
                }
            }
            else
            {
                provider = new OobProvider();    //If no custom one is set just use OOB one
            }
            
            return provider;
        }
    }
}
