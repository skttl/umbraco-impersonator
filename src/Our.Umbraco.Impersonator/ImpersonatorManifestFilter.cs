using System.Collections.Generic;
using Umbraco.Cms.Core.Manifest;

namespace Our.Umbraco.Impersonator
{
    public class ImpersonatorManifestFilter : IManifestFilter
    {
        public void Filter(List<PackageManifest> manifests)
        {
            manifests.Add(new PackageManifest()
            {
                PackageName = "Our.Umbraco.Impersonator",
                AllowPackageTelemetry = true,
                Dashboards = new[]
                {
                    new ManifestDashboard()
                    {
                        Alias = "impersonator",
                        Sections = new[]
                        {
                            "user-dialog"
                        },
                        View = "/App_Plugins/Our.Umbraco.Impersonator/user.html",
                        Weight = 0
                    }
                },
                Scripts = new[]
                {
                    "/App_Plugins/Our.Umbraco.Impersonator/user.controller.js"
                }
            });
        }
    }
}
