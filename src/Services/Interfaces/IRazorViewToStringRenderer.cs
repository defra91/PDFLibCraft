// <copyright file="IRazorViewToStringRenderer.cs" company="Luca De Franceschi">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace PDFLibCraft.Services.Interfaces;

using System.Threading.Tasks;

/// <summary>
/// Represents the interface of the service for rendering the Razor Views into string.
/// </summary>
public interface IRazorViewToStringRenderer
{
    /// <summary>
    /// Given a view name and the view model render the view into string.
    /// </summary>
    /// <typeparam name="TViewModel">The view model to work on.</typeparam>
    /// <param name="viewName">The view name.</param>
    /// <param name="model">The view model object.</param>
    /// <returns>Task resulting into the rendered string.</returns>
    public Task<string> RenderViewToStringAsync<TViewModel>(string viewName, TViewModel model);
}