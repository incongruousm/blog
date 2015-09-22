---
layout: post
title:  "WPF Designer & Custom Dependency Properties"
date:   2014-05-30
tags: tech wpf dotnet
---

We needed a custom WPF layout for a work project so we merrily set about extending [Panel](http://msdn.microsoft.com/en-us/library/system.windows.controls.panel%28v=vs.110%29.aspx) and implementing our [MeasureOverride](http://msdn.microsoft.com/en-us/library/system.windows.frameworkelement.measureoverride%28v=vs.110%29.aspx) and [ArrangeOverride](http://msdn.microsoft.com/en-us/library/system.windows.frameworkelement.arrangeoverride%28v=vs.110%29.aspx) methods. However, we had some issues with the designer not responding well to XAML changes for our custom dependency properties. The solution turned out to be simple. Register the property with [FrameworkPropertyMetadata](http://msdn.microsoft.com/en-us/library/system.windows.frameworkpropertymetadata%28v=vs.110%29.aspx) instead of [PropertyMetadata](http://msdn.microsoft.com/en-us/library/system.windows.propertymetadata%28v=vs.110%29.aspx) and set the [AffectsArrange](http://msdn.microsoft.com/en-us/library/system.windows.frameworkpropertymetadata.affectsarrange%28v=vs.110%29.aspx) and [AffectsMeasure](http://msdn.microsoft.com/en-us/library/system.windows.frameworkpropertymetadata.affectsmeasure%28v=vs.110%29.aspx) flags.

```csharp
static DependencyProperty MyCustomMarginProperty
         = DependencyProperty.Register("MyCustomMargin",
                                       typeof (double),
                                       typeof (MyControl),
                                       new FrameworkPropertyMetadata(default(double))
                                          {
                                             AffectsArrange = true,
                                             AffectsMeasure = true
                                          });
```