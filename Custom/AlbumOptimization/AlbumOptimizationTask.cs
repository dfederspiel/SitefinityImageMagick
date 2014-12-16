using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Modules.Libraries;
using Telerik.Sitefinity.Scheduling;
using Telerik.Sitefinity.Workflow;
using Telerik.Sitefinity.Model;
using ImageMagick;

namespace SitefinityWebApp.Custom.AlbumOptimization
{
	public class AlbumOptimizationTask : ScheduledTask
	{
		private int _itemsCount;

		private int _currentIndex;

		public Guid AlbumId
		{
			get;
			set;
		}

		public override string TaskName
		{
			get
			{
				return "SitefinityWebApp.Custom.AlbumOptimization.AlbumOptimizationTask";
			}
		}

		public AlbumOptimizationTask()
		{
			Title = "OptimizeAlbum";
			ExecuteTime = DateTime.UtcNow;
			Description = "Optimizing images";
		}

		public override string BuildUniqueKey()
		{
			return this.GetCustomData();
		}

		public override void ExecuteTask()
		{
			LibrariesManager _librariesManager = LibrariesManager.GetManager();

			Album album = _librariesManager.GetAlbum(this.AlbumId);

			var albumProvider = (LibrariesDataProvider)album.Provider;

			var images = album.Images().Where(i => i.Status == ContentLifecycleStatus.Master && !i.GetValue<bool>("Optimized"));
			_itemsCount = images.Count();

			AlbumOptimizationConfig _albumOptimaztionConfig = Config.Get<AlbumOptimizationConfig>();
			int _imageQuality = 100;
			if (_albumOptimaztionConfig.ImageQuality != null)
			{
				_imageQuality = _albumOptimaztionConfig.ImageQuality;
			}

			foreach (Telerik.Sitefinity.Libraries.Model.Image image in images)
			{
				// Pull the Stream of the image from the provider.
				// This saves us from having to care about BlobStorage
				Stream imageData = albumProvider.Download(image);
				
				using (MemoryStream compressed = new MemoryStream())
				{
					MagickReadSettings magickSettings = new MagickReadSettings();
					switch (image.Extension)
					{
						case ".png":
							magickSettings.Format = MagickFormat.Png;
							break;
						case ".jpg":
							magickSettings.Format = MagickFormat.Jpg;
							break;
						case ".jpeg":
							magickSettings.Format = MagickFormat.Jpeg;
							break;
						case ".bmp":
							magickSettings.Format = MagickFormat.Bmp;
							break;
						default:
							magickSettings.Format = MagickFormat.Jpg;
							break;
					}
					using (MagickImage img = new MagickImage(imageData, magickSettings))
					{
						img.Quality = _imageQuality;
						img.Write(compressed);
						if (compressed == null)
						{
							UpdateProgress();
							continue;
						}

						//Check out the master to get a temp version.
						Image temp = _librariesManager.Lifecycle.CheckOut(image) as Telerik.Sitefinity.Libraries.Model.Image;

						//Make the modifications to the temp version.
						_librariesManager.Upload(temp, compressed, image.Extension);

						temp.SetValue("Optimized",true);
						//Checkin the temp and get the updated master version.
						//After the check in the temp version is deleted.
						_librariesManager.Lifecycle.CheckIn(temp);

						_librariesManager.SaveChanges();

						// Check to see if this image is already published.
						// If it is, we need to publish the "Master" to update "Live"
						if (image.GetWorkflowState() == "Published")
						{
							var bag = new Dictionary<string, string>();
							bag.Add("ContentType", typeof(Telerik.Sitefinity.Libraries.Model.Image).FullName);
							WorkflowManager.MessageWorkflow(image.Id, typeof(Telerik.Sitefinity.Libraries.Model.Image), albumProvider.Name, "Publish", false, bag);
						}
					}

				}

				UpdateProgress();
			}

		}


		private void UpdateProgress()
		{
			AlbumOptimizationTask albumOptimizationTask = this;
			albumOptimizationTask._currentIndex = albumOptimizationTask._currentIndex + 1;
			TaskProgressEventArgs taskProgressEventArg = new TaskProgressEventArgs()
			{
				Progress = this._currentIndex * 100 / this._itemsCount,
				StatusMessage = ""
			};
			TaskProgressEventArgs taskProgressEventArg1 = taskProgressEventArg;
			this.OnProgressChanged(taskProgressEventArg1);
			if (taskProgressEventArg1.Stopped)
			{
				throw new TaskStoppedException();
			}
		}

		public override string GetCustomData()
		{
			AlbumOptimizationTaskState albumOptimizationTaskState = new AlbumOptimizationTaskState(this);
			return JsonConvert.SerializeObject(albumOptimizationTaskState);
		}

		public override void SetCustomData(string customData)
		{
			AlbumOptimizationTaskState albumOptimizationTaskState = JsonConvert.DeserializeObject<AlbumOptimizationTaskState>(customData);
			this.AlbumId = albumOptimizationTaskState.AlbumId;
		}
	}

	internal class AlbumOptimizationTaskState
	{
		public Guid AlbumId
		{
			get;
			set;
		}

		public AlbumOptimizationTaskState()
		{
		}

		public AlbumOptimizationTaskState(AlbumOptimizationTask albumOptimizationTask)
		{
			this.AlbumId = albumOptimizationTask.AlbumId;
		}
	}
}