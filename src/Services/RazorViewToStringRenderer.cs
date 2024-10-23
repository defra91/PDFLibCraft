// <copyright file="RazorViewToStringRenderer.cs" company="Luca De Franceschi">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PDFLibCraft.Services;

using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using PDFLibCraft.Services.Interfaces;

/// <summary>
/// Code from: https://github.com/aspnet/Entropy/blob/dev/samples/Mvc.RenderViewToString/RazorViewToStringRenderer.cs
/// Implementation of the <see cref="IRazorViewToStringRenderer"/> interface.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RazorViewToStringRenderer"/> class.
/// </remarks>
/// <param name="viewEngine">The view engine reference.</param>
/// <param name="tempDataProvider">The temp data provider reference.</param>
/// <param name="serviceProvider">The service provider reference.</param>
public class RazorViewToStringRenderer(
    IRazorViewEngine viewEngine,
    ITempDataProvider tempDataProvider,
    IServiceProvider serviceProvider)
    : IRazorViewToStringRenderer
{
    private readonly IRazorViewEngine viewEngine = viewEngine;
    private readonly ITempDataProvider tempDataProvider = tempDataProvider;
    private readonly IServiceProvider serviceProvider = serviceProvider;

    /// <inheritdoc />
    public async Task<string> RenderViewToStringAsync<TViewModel>(string viewName, TViewModel model)
    {
        var actionContext = this.GetActionContext();
        var view = this.FindView(actionContext, viewName);

        using var output = new StringWriter();
        var viewContext = new ViewContext(
            actionContext,
            view,
            new ViewDataDictionary<TViewModel>(
                metadataProvider: new EmptyModelMetadataProvider(),
                modelState: new ModelStateDictionary())
            {
                Model = model,
            },
            new TempDataDictionary(
                actionContext.HttpContext,
                this.tempDataProvider),
            output,
            new HtmlHelperOptions());

        await view.RenderAsync(viewContext);

        return output.ToString();
    }

    private IView FindView(ActionContext actionContext, string viewName)
    {
        var getViewResult = this.viewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
        if (getViewResult.Success)
        {
            return getViewResult.View;
        }

        var findViewResult = this.viewEngine.FindView(actionContext, viewName, isMainPage: true);
        if (findViewResult.Success)
        {
            return findViewResult.View;
        }

        var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
        var errorMessage = string.Join(
            Environment.NewLine,
            new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations));

        throw new InvalidOperationException(errorMessage);
    }

    private ActionContext GetActionContext()
    {
        var httpContext = new DefaultHttpContext();
        httpContext.RequestServices = this.serviceProvider;
        return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
    }
}