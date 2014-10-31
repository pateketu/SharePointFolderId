<?xml version="1.0" encoding="utf-8"?>
<feature xmlns:dm0="http://schemas.microsoft.com/VisualStudio/2008/DslTools/Core" dslVersion="1.0.0.0" Id="fef0c573-7595-4b9f-bf0c-693fd572852b" description="Assign Folder Ids to Folders based on OOB Document Id Service" featureId="fef0c573-7595-4b9f-bf0c-693fd572852b" imageUrl="" receiverAssembly="$SharePoint.Project.AssemblyFullName$" receiverClass="$SharePoint.Type.33aae70a-2461-4f7a-8fa3-d99e5f42485d.FullName$" scope="Site" solutionId="00000000-0000-0000-0000-000000000000" title="Folder Id Service" version="" deploymentPath="$SharePoint.Project.FileNameWithoutExtension$_$SharePoint.Feature.FileNameWithoutExtension$" xmlns="http://schemas.microsoft.com/VisualStudio/2008/SharePointTools/FeatureModel">
  <activationDependencies>
    <customFeatureActivationDependency minimumVersion="" featureTitle="Document Id Service" featureDescription="Document Id Service must be active on the site collection for Folder Id Service to work" featureId="b50e3104-6812-424f-a011-cc90e6327318" solutionId="00000000-0000-0000-0000-000000000000" solutionUrl="" />
  </activationDependencies>
  <projectItems>
    <projectItemReference itemId="c8713ade-8e02-43d0-a737-63c8ba76a0fc" />
    <projectItemReference itemId="8745bb3a-c830-44bc-8210-62e38abfb26e" />
  </projectItems>
</feature>