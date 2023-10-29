using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MauiApiRest.Models;
using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;

namespace MauiApiRest.ViewModels;

public partial class MainViewModel : ObservableObject
{
    #region Propriedades
    HttpClient cliente;
    JsonSerializerOptions _serializerOptions;
    string baseUrl = "https://catalogo.macoratti.net/api/1";

    [ObservableProperty]
    public string _categoriaInfoId;

    [ObservableProperty]
    public string _categoriaInfoNome;

    [ObservableProperty]
    public Categoria _categoria;

    [ObservableProperty]
    public ObservableCollection<Categoria> _categorias;

    [ObservableProperty]
    private string _nome;

    [ObservableProperty]
    private string _imagemUrl; 
    #endregion

    public MainViewModel()
    {
        cliente = new HttpClient();
        Categorias = new ObservableCollection<Categoria>();
        _serializerOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    [RelayCommand]
    public async Task GetCategorias()
    {
        var url = $"{baseUrl}/categorias";

        var response = await cliente.GetAsync(url);

        if(response.IsSuccessStatusCode)
        {
            using (var responseStream = await response.Content.ReadAsStreamAsync())
            {
                var data = await JsonSerializer.DeserializeAsync<ObservableCollection<Categoria>>(responseStream, _serializerOptions);
                Categorias = data;
            }
        }
    }

    [RelayCommand]
    public async Task GetCategoria()
    {
        if(CategoriaInfoId is not null)
        {
            var categoriaId = int.Parse(CategoriaInfoId);

            var url = $"{baseUrl}/categorias/{categoriaId}";

            var response = await cliente.GetAsync(url);

            if(response.IsSuccessStatusCode)
            {
                using (var responseStream = await response.Content.ReadAsStreamAsync())
                {
                    var data = await JsonSerializer.DeserializeAsync<Categoria>(responseStream, _serializerOptions);

                    Categoria = data;
                }
            }
        }
    }

    [RelayCommand]
    public async Task AddCategoria()
    {
        var url = $"{baseUrl}/categorias";

        if(CategoriaInfoNome is not null)
        {
            var categoria = new Categoria
            {
                Nome = CategoriaInfoNome,
                ImagemUrl = "https://www.macoratti.net/Imagens/lanches/pudim1.jpg" //exemplo de imagem
            };

            string json = JsonSerializer.Serialize<Categoria>(categoria, _serializerOptions);

            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await cliente.PostAsync(url, content);

            await GetCategorias();
        }
    }

    [RelayCommand]
    public async Task UpdateCategoria()
    {
        if (CategoriaInfoId is not null)
        {
            var categoiraId = int.Parse(CategoriaInfoId);

            var categoria = Categorias.FirstOrDefault(x => x.CategoriaId == categoiraId);

            var url = $"{baseUrl}/categorias/{categoiraId}";

            categoria.Nome = CategoriaInfoNome;

            string jsonResponse = JsonSerializer.Serialize<Categoria>(categoria, _serializerOptions);

            StringContent content = new StringContent(jsonResponse, Encoding.UTF8, "application/json");

            var response = await cliente.PutAsync(url, content);
            await GetCategorias();
        }
    }

    [RelayCommand]
    public async Task DeleteCategoria()
    {
        if(CategoriaInfoId is not null)
        {
            var categoriaid = int.Parse(CategoriaInfoId);

            if(categoriaid > 0)
            {
                var url = $"{baseUrl}/categorias/{categoriaid}";

                var response = await cliente.DeleteAsync(url);

                await GetCategorias();
            }
        }
    }
}
