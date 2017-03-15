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
      Assert.ArgumentNotNull(httpContext, nameof(httpContext));
      Assert.ArgumentNotNull(visit, nameof(visit));
      
      IPageContext context = visit.CreatePage();
      Assert.IsNotNull(context, nameof(context));
      
      var currentPage = WebUtil.CurrentPage;
      Assert.IsNotNull(currentPage, nameof(currentPage));
      
      var request = currentPage.Request;
      Assert.IsNotNull(request, nameof(request));
      
      var rawUrl = request.Url.AbsoluteUri;
      Assert.IsNotNull(rawUrl, nameof(rawUrl));
      
      context.SetUrl(rawUrl);
      
      var contextDevice = context.SitecoreDevice;
      Assert.IsNotNull(contextDevice, nameof(contextDevice));
      
      var device = Context.Device;
      if (device != null)
      {
        contextDevice.Id = device.ID.Guid;
        contextDevice.Name = device.Name;
      }
      else
      {
        contextDevice.Id = Guid.Empty;
        contextDevice.Name = string.Empty;
      }
      
      var item = Context.Item;
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
