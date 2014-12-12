using SitefinityWebApp.Custom.AlbumOptimization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Scheduling;
using Telerik.Sitefinity.Workflow;

namespace SitefinityWebApp.Mvc.Controllers
{
    public class OptimizationController : ApiController
    {


        [Authorize]
        [HttpPost]
        public Guid Post(Guid id)
        {
            return StartOptimizeAlbumItemsTask(id);
        }

        private Guid StartOptimizeAlbumItemsTask(Guid albumId)
        {
            SchedulingManager manager = SchedulingManager.GetManager();

            Guid guid = Guid.NewGuid();

            AlbumOptimizationTask albumOptimizationTask = new AlbumOptimizationTask()
            {
                Id = guid,
                AlbumId = albumId
            };

            manager.AddTask(albumOptimizationTask);

            manager.SaveChanges();

            return guid;
        }

    }
}