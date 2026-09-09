using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PI_T1.ViewModels;

/// <summary>
/// Classe base para ViewModels que implementa a interface INotifyPropertyChanged.
/// Fornece infraestrutura para notificação reativa de alteração de propriedades no padrão MVVM do WPF.
///
/// Documentação oficial de referência:
/// - Microsoft Learn (Como implementar a notificação de alteração de propriedade):
///   https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/data/how-to-implement-property-change-notification
/// - Microsoft Learn (Visão Geral do Data Binding no WPF):
///   https://learn.microsoft.com/pt-br/dotnet/desktop/wpf/data/data-binding-overview
/// </summary>
public abstract class ViewModelBase : INotifyPropertyChanged
{
    //# =======================================================================
    //# EVENTO DE NOTIFICAÇÃO DO DATA BINDING
    //# =======================================================================

    /// <summary>
    /// Ocorre quando o valor de uma propriedade observável é modificado.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Dispara o evento PropertyChanged para a propriedade informada via nome (aloca PropertyChangedEventArgs).
    /// </summary>
    /// <param name="propertyName">Nome da propriedade modificada (capturado automaticamente via CallerMemberName).</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    /// Dispara o evento PropertyChanged utilizando uma instância pré-alocada em cache (Zero Allocations).
    /// </summary>
    /// <param name="args">Instância em cache de PropertyChangedEventArgs.</param>
    protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
    {
        PropertyChanged?.Invoke(this, args);
    }

    /// <summary>
    /// Atualiza o campo de suporte e dispara a notificação caso o valor tenha se alterado.
    /// </summary>
    /// <typeparam name="T">Tipo do dado armazenado.</typeparam>
    /// <param name="field">Referência para a variável de suporte privada.</param>
    /// <param name="value">Novo valor a ser atribuído.</param>
    /// <param name="propertyName">Nome da propriedade alterada.</param>
    /// <returns>True se o valor foi alterado; False caso contrário.</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }

    /// <summary>
    /// Atualiza o campo de suporte e dispara a notificação com argumentos em cache, eliminando coletas de lixo (Zero Alloc).
    /// </summary>
    /// <typeparam name="T">Tipo do dado armazenado.</typeparam>
    /// <param name="field">Referência para a variável de suporte privada.</param>
    /// <param name="value">Novo valor a ser atribuído.</param>
    /// <param name="args">Instância em cache de PropertyChangedEventArgs.</param>
    /// <returns>True se o valor foi alterado; False caso contrário.</returns>
    protected bool SetProperty<T>(ref T field, T value, PropertyChangedEventArgs args)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;
        OnPropertyChanged(args);
        return true;
    }
}
