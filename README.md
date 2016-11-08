# SupUrlative

Links and templates are at the heart of a [Hypermedia](http://apievangelist.com/2014/01/07/what-is-a-hypermedia-api/) API. It is important to generate correct resource links in addition to properly defining the capabilities each resources exposes via url templates. ASP.NET WebAPI comes with first class support for generating urls using the `RouteCollection` class. **SupUrlative** attempts to standardize the way urls and templates are created by utilizing the routes, request objects, and Nuget libraries you already use when building a WebAPI.

## Getting Started

To get started using **SupUrlative**, first install the library from Nuget using the following command.

```
PM> Install-Package RimDev.Supurlative
```

You may optionally add the **Paging** extension if you are going to use **any** PagedList Nuget package.

```
PM> Install-Package RimDev.Supurlative.Paging
```

Once the package is installed, you have a few classes you may use to generate links and templates. The classes you will use are the following:

1. SuperlativeOptions
2. TemplateGenerator
3. UrlGenerator
3. Generator (TemplateGenerator &amp; UrlGenerator)
4. PagingGenerator (Generator + Paging)

## SupurlativeOptions

The options class allows you to control the behavior of your generator through several properties:

1. **UriKind** - Absolute or Relative url generatorOptions
2. **PropertyNameSeperator** - When dealing with complex objects, the seperator that appears in the parameter name
3. **Formatters** - a collection of global formatters
4. **LowercaseKeys** - Determines whether the url keys will retain their casing, or be lowercased. Defaults to `true`.

Use this class to set behavior of the generator it is passed in to.

## TemplateGenerator

The `TemplateGenerator` class will generate templates that are modeled after the [URI Template Spec RFC6570](http://tools.ietf.org/html/rfc6570). You may use [Tavis.UriTemplate](https://github.com/tavis-software/Tavis.UriTemplates) to resolve templates.

Below you will find an example of using the `TemplateGenerator` class.

```csharp
// we need the HttpRequestMessage to get the
// RoutesCollection and other request values
// like the domain.
var generator = new TemplateGenerator(httpRequestMessage);

// http://localhost:8000/foo/{id}
var actual = generator.Generate("foo.show");
```

The above example is very simple, but the template generator can also take into account querystring options by utilizing a request model.

```csharp
var generator = new TemplateGenerator(httpRequestMessage);
// http://localhost:8000/foo/{id}{?bar};
generator.Generate("foo.show", new { Id = 1, Bar = "Foo" });
```

The template generator can handle nested complex models, and as you will see later can also allow the developer to intercept the key/value creation process and modify the templates as needed.

## UrlGenerator

The `UrlGenerator` class utilizes a request model to generate a url from within the context of a WebAPI request. This is route resolution on steroids, thanks to the addition of `Formatters` which you will see in a following section.

```csharp
var generator = new UrlGenerator(httpRequestMessage);
// http://localhost:8000/1
generator.Generate("foo.show", new { Id = 1 });
```

The url generator can also take into account queryString parameters.

```csharp
var generator = new UrlGenerator(httpRequestMessage);
// http://localhost:8000/i?foo=yes
generator.Generate("foo.show", new { Id = 1, Foo = "yes" });
```

## Generator

The generator class combines bot the `TemplateGenerator` and `UrlGenerator` classes. Most often you will want to generate both the current request as a `self` link, along with a template url for the current resource.

The interface is similar to the generators this class wraps.

```csharp
var generator = new Generator(httpRequestMessage);
var result = generator.Generate("routeName", request);

result.Url;
result.Template;
```

## Formatters

Formatters allow the developer to intercept the url/template creation process and inject any value they would like into the process. Formatters can be applied either as an attribute on the property directly, or globally at the `SupurlativeOptions` level. Here is an example of a `CoordinateFormatter`:

```csharp
public class CoordinateFormatter : BaseFormatterAttribute
{
    public override void Invoke(string fullPropertyName, object value, Type valueType, IDictionary<string, object> dictionary, SupurlativeOptions options)
    {
        var coordinates = value as LocationRequest.Coordinate;
        dictionary.Add(fullPropertyName, coordinates == null ? null : coordinates.ToString());
    }

    public override bool IsMatch(Type currentType, SupurlativeOptions options)
    {
        return IsMatch(typeof(LocationRequest.Coordinate), currentType, options);
    }
}
```

Three things to note about a `Formatter`:

1. It is an attribute.
2. The `IsMatch` method is crucial to executing the `Invoke` method.
3. The `Invoke` has a dictionary, which makes the formatter in charge of adding the key/value pair to the url/template generation process.

Let's look at how the formatter executes and alters the resulting url.

```csharp
// http://localhost:8000/place?location=0-0";
Generator.Generate("location", new LocationRequest
{
    Location = new LocationRequest.Coordinate
    {
        X = 0,
        Y = 0
    }
});
```

Note, the resulting url has no reference to the properties `X` and `Y` because the `CoordinateFormatter` took care of the key and value.

## Paging

While paging is optional, it can be advantageous to use the library to generate your `next` and `previous` urls. The urls can be used on resources that expose data in a paged fashion.

```csharp
var result = Generator.Generate("page.query",
 new Request { page = 1 },
 // PagedList result set resulting
 // from the current request
 PagedList,
 // expression to the page property
 x => x.page);

// http://localhost:8000/paging?page=2
result.NextUrl;
```

The `PagedList` property can have one of two contracts:

The first is the common PagedList interface found in many common PagedList
implementations.

```csharp
public interface IPagedList
{
    int PageNumber { get; }
    int PageSize { get; }
    int TotalItemCount { get; }
}
```

The second interface is something proprietary to the Supurlative package.

```csharp
public interface IPaged
{
    int? Page { get; }
    int? PageSize { get; }
    int? TotalItemCount { get; }
}
```

Note: You **do not** have to explicitly implement these interfaces on your types, you
just have to have these properties on the parameter object you are passing
to the `Generate` method.

When generating paging urls, you will get a `PagingResult` from the generator. The following are the properties you will have access from the result.

1. Template
2. Url
3. NextUrl
4. HasNextUrl
5. PreviousUrl
6. HasPreviousUrl

## Notes

- This library is still in the early stages, and could use more vetting and more changes may be coming.
- Not all templating scenarios have been explored, but we believe they can be achieved using `Formatters`
- This library supports route constraints.
- This library works with AttributeRouting found in WebAPI.
- Look at the unit tests to see more examples.
