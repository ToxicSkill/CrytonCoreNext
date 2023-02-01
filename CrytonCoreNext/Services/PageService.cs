using CrytonCoreNext.Interfaces;
using System;
using System.Windows;

namespace CrytonCoreNext.Services;

public class PageService : ICustomPageService
{
    /// <summary>
    /// Service which provides the instances of pages.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    public event EventHandler<string> OnPageNavigate;

    /// <summary>
    /// Creates new instance and attaches the <see cref="IServiceProvider"/>.
    /// </summary>
    public PageService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public T? GetPage<T>() where T : class
    {
        if (!typeof(FrameworkElement).IsAssignableFrom(typeof(T)))
            throw new InvalidOperationException("The page should be a WPF control.");

        return (T?)_serviceProvider.GetService(typeof(T));
    }

    bool e = false;

    /// <inheritdoc />
    public FrameworkElement? GetPage(Type pageType)
    {
        if (!typeof(FrameworkElement).IsAssignableFrom(pageType))
            throw new InvalidOperationException("The page should be a WPF control.");
        OnPageNavigate.Invoke(null, pageType.Name);
        //if (pageType == typeof(PdfMergeView) && !e)
        //{
        //    e = !e;
        //    return GetPage(typeof(NavigationPDFView));
        //}
        //if (pageType == typeof(NavigationPDFView) && e)
        //{
        //    return GetPage(typeof(PdfMergeView));
        //}
        return _serviceProvider.GetService(pageType) as FrameworkElement;
    }
}
