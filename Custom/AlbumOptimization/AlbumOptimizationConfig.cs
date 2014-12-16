using System;
using System.Configuration;
using System.Linq;
using Telerik.Sitefinity.Configuration;
using Telerik.Sitefinity.Localization;

namespace SitefinityWebApp.Custom.AlbumOptimization
{
	/// <summary>
	/// Sitefinity configuration section.
	/// </summary>
	/// <remarks>
	/// If this is a Sitefinity module's configuration,
	/// you need to add this to the module's Initialize method:
	/// App.WorkWith()
	///     .Module(ModuleName)
	///     .Initialize()
	///         .Configuration<AlbumOptimizationConfig>();
	/// 
	/// You also need to add this to the module:
	/// protected override ConfigSection GetModuleConfig()
	/// {
	///     return Config.Get<AlbumOptimizationConfig>();
	/// }
	/// </remarks>
	/// <see cref="http://www.sitefinity.com/documentation/documentationarticles/developers-guide/deep-dive/configuration/creating-configuration-classes"/>
	[ObjectInfo(Title = "Album Optimization Config", Description = "Use this to set teh quality for image optimazation")]
	public class AlbumOptimizationConfig : ConfigSection
	{
		[ObjectInfo(Title = "Image Quality", Description = "Input a number 0-100 for the quality of image optimization 0 being the lowest quality")]
		[ConfigurationProperty("ImageQuality", DefaultValue = "70")]
		public int ImageQuality
		{
			get
			{
				return (int)this["ImageQuality"];
			}
			set
			{
				this["ImageQuality"] = value;
			}
		}
	}
}