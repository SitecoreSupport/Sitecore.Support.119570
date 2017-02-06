using Sitecore.Analytics.Pipelines.InitializeTracker;
using Sitecore.Analytics.Tracking;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Web;
using System;
using System.Web;

namespace Sitecore.Support.Analytics.Pipelines.InitializeTracker
{
  public class CreatePage : InitializeTrackerProcessor
  {
    private void CreateAndInitializePage(HttpContextBase httpContext, CurrentInteraction visit)
    {
      IPageContext context = visit.CreatePage();
      string rawUrl = WebUtil.CurrentPage.Request.Url.AbsoluteUri;
      context.SetUrl(rawUrl);
      DeviceItem device = Context.Device;
      if (device != null)
      {
        context.SitecoreDevice.Id = device.ID.Guid;
        context.SitecoreDevice.Name = device.Name;
      }
      else
      {
        context.SitecoreDevice.Id = Guid.Empty;
        context.SitecoreDevice.Name = string.Empty;
      }
      Item item = Context.Item;
      if (item != null)
      {
        context.SetItemProperties(item.ID.Guid, item.Language.Name, item.Version.Number);
      }
    }

    public override void Process(InitializeTrackerArgs args)
    {
      Assert.ArgumentNotNull(args, "args");
      if (!args.IsSessionEnd)
      {
        HttpContextBase httpContext = args.HttpContext;
        if (httpContext == null)
        {
          args.AbortPipeline();
        }
        else
        {
          this.CreateAndInitializePage(httpContext, args.Session.Interaction);
        }
      }
    }

    // Properties
    [Obsolete("This property is no longer used.")]
    public int MaxPageIndexThreshold { get; set; }

  }
}