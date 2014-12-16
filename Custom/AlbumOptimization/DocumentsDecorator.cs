using System;
using System.Linq;
using System.Text.RegularExpressions;
using Telerik.Sitefinity.GenericContent.Model;
using Telerik.Sitefinity.Libraries.Model;
using Telerik.Sitefinity.Lifecycle;
using Telerik.Sitefinity.Model;
using Telerik.Sitefinity.Security.Claims;

namespace Aptera.Sitefinity.Decorators
{
	public class DocumentsDecorator : LifecycleDecorator
	{
		public DocumentsDecorator(ILifecycleManager manager, LifecycleItemCopyDelegate copyDelegate, params Type[] itemTypes)
			: base(manager, copyDelegate, itemTypes)
		{
		}

		public DocumentsDecorator(ILifecycleManager manager, Action<Content, Content> copyDelegate, params Type[] itemTypes)
			: base(manager, copyDelegate, itemTypes)
		{
		}

		protected override ILifecycleDataItemGeneric ExecuteOnPublish(ILifecycleDataItemGeneric masterItem, ILifecycleDataItemGeneric liveItem, System.Globalization.CultureInfo culture = null, DateTime? publicationDate = null)
		{
			var identity = ClaimsManager.GetCurrentIdentity();
			var userName = identity.Name;

			if (masterItem is Image)
			{
				var imageItem = masterItem as Image;
				if (liveItem != null && userName != "system")
				{
					imageItem.SetValue("Optimized",false);
				}
				return base.ExecuteOnPublish(imageItem as ILifecycleDataItemGeneric, liveItem, culture, publicationDate);
			}
			else
			{
				return base.ExecuteOnPublish(masterItem, liveItem, culture, publicationDate);
			}
		}
	}
}
