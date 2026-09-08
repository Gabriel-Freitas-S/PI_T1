# URLs oficiais — Microsoft Learn (WPF/XAML + XML .NET)

> Base: `https://learn.microsoft.com/{locale}/...` com `locale = pt-br` (preferencial) ou `en-us` (fallback).
> Todas dentro do allowlist: `learn.microsoft.com` apenas.

## WPF — visao geral e getting started

- Visao geral do WPF: https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/
- Introducao ao WPF: https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/getting-started/
- O que e XAML: https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/xaml/
- Sintaxe XAML em detalhes: https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/advanced/xaml-syntax-in-detail/
- Estrutura de aplicativo WPF: https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/app-development/

## WPF — layout e controles (corpo da locomotiva, Canvas)

- Layout no WPF: https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/layout/
- Elemento Canvas: https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/canvas/
- Formas basicas (Rectangle, Ellipse, Polygon, Line): https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/shapes-and-basic-drawing-in-wpf-overview/
- Controles WPF (visao geral): https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/control-library/

## WPF — transforms, animacao, storyboard (rodas + translacao)

- Visao geral de transformacoes: https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview/
- RotateTransform: https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.rotatetransform/
- TranslateTransform: https://learn.microsoft.com/pt-br/dotnet/api/system.windows.media.translatetransform/
- Visao geral de animacao (Storyboard, DoubleAnimation): https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/animation-overview/
- Como animar com Storyboard: https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/storyboards-overview/
- RenderTransform vs LayoutTransform: https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/graphics-multimedia/transforms-overview/

## WPF — templates, estilos, data binding (ControlTemplate da roda)

- Estilos e templates: https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/styles-templates-overview/
- Modelos de controle (ControlTemplate): https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/controls/controltemplates-overview/
- Data binding (visao geral): https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/data/data-binding-overview/
- Recursos XAML (StaticResource/DynamicResource): https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/systems/xaml-resources/

## XML no .NET (se o trabalho precisar persistir/configurar via XML)

- XML no .NET (guia): https://learn.microsoft.com/pt-br/dotnet/standard/data/xml/
- XmlReader: https://learn.microsoft.com/pt-br/dotnet/api/system.xml.xmlreader/
- XmlWriter: https://learn.microsoft.com/pt-br/dotnet/api/system.xml.xmlwriter/
- LINQ to XML (XDocument): https://learn.microsoft.com/pt-br/dotnet/standard/linq/linq-xml-overview/
- XmlSerializer: https://learn.microsoft.com/pt-br/dotnet/standard/serialization/xml-and-soap-serialization/
- XPathNavigator / XPath: https://learn.microsoft.com/pt-br/dotnet/standard/data/xml/xpath/

## Regra de fallback

1. Tente `pt-br` primeiro.
2. Se vazio/404, troque para `en-us`, ex.:
   `.../pt-br/dotnet/desktop/wpf/xaml/` -> `.../en-us/dotnet/desktop/wpf/xaml/`
