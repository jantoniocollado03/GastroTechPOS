using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using GastroTechPOS.Infrastructure.Repositories;
using GestorTechPOS.Models;
using GestorTechPOS.Services;

namespace GastroTechPOS.UI.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly RepositorioComandasMongo _repoComandas;
    private readonly RepositorioMesasMongo _repoMesas;
    private readonly RepositorioCajaMongo _repoCaja;
    private readonly RepositorioProductosMongo _repoProductos;
    private readonly ServicioTPV _servicio;

    public string[] TiposProducto { get; } = { "Bebida", "Plato principal", "Postre" };
    public string[] FiltrosCategoria { get; } = { "Todos", "Bebidas", "Platos", "Postres" };

    [ObservableProperty]
    private ObservableCollection<Mesa> mesas = new();

    [ObservableProperty]
    private Mesa? mesaSeleccionada;

    [ObservableProperty]
    private ObservableCollection<Producto> productos = new();

    [ObservableProperty]
    private ObservableCollection<Producto> productosFiltrados = new();

    [ObservableProperty]
    private Producto? productoSeleccionado;

    [ObservableProperty]
    private Producto? productoSeleccionadoParaComanda;

    [ObservableProperty]
    private ObservableCollection<string> ticketsCaja = new();

    [ObservableProperty]
    private string mensaje = "Preparando conexión con MongoDB...";

    [ObservableProperty]
    private string numeroNuevaMesa = "";

    [ObservableProperty]
    private string nombreProducto = "";

    [ObservableProperty]
    private string precioProducto = "";

    [ObservableProperty]
    private string tipoProductoSeleccionado = "Bebida";

    [ObservableProperty]
    private bool productoTieneAlcohol;

    [ObservableProperty]
    private bool productoEsCaliente;

    [ObservableProperty]
    private bool productoTieneLactosa;

    [ObservableProperty]
    private string cantidadProducto = "1";

    [ObservableProperty]
    private string filtroCategoria = "Todos";

    [ObservableProperty]
    private decimal totalCaja;

    [ObservableProperty]
    private int cantidadVentas;

    public string TituloMesaSeleccionada => MesaSeleccionada is null
        ? "Selecciona una mesa"
        : $"Mesa {MesaSeleccionada.Numero} - {MesaSeleccionada.Estado}";

    public string TotalComandaTexto
    {
        get
        {
            if (MesaSeleccionada?.Comanda is null)
                return "Total comanda: 0,00 €";

            return $"Total comanda: {MesaSeleccionada.Comanda.CalcularTotal():0.00} €";
        }
    }

    public string TiempoPreparacionTexto
    {
        get
        {
            if (MesaSeleccionada?.Comanda is null || !MesaSeleccionada.Comanda.Lineas.Any())
                return "Tiempo preparación: 0 min";

            int minutos = MesaSeleccionada.Comanda.Lineas
                .Sum(linea => linea.Producto.TiempoPreparacion() * linea.Cantidad);

            return $"Tiempo preparación: {minutos} min";
        }
    }

    public MainWindowViewModel()
    {
        _repoComandas = new RepositorioComandasMongo();
        _repoMesas = new RepositorioMesasMongo();
        _repoCaja = new RepositorioCajaMongo();
        _repoProductos = new RepositorioProductosMongo();
        _servicio = new ServicioTPV(_repoComandas, _repoMesas, _repoCaja);

        CargarDatosDesdeMongo();
    }

    partial void OnMesaSeleccionadaChanged(Mesa? value)
    {
        OnPropertyChanged(nameof(TituloMesaSeleccionada));
        OnPropertyChanged(nameof(TotalComandaTexto));
        OnPropertyChanged(nameof(TiempoPreparacionTexto));
    }

    partial void OnFiltroCategoriaChanged(string value)
    {
        AplicarFiltroProductos();
    }

    partial void OnTipoProductoSeleccionadoChanged(string value)
    {
        if (value == "Bebida")
        {
            ProductoEsCaliente = false;
            ProductoTieneLactosa = false;
        }
        else if (value == "Plato principal")
        {
            ProductoTieneAlcohol = false;
            ProductoTieneLactosa = false;
        }
        else if (value == "Postre")
        {
            ProductoTieneAlcohol = false;
        }
    }

    [RelayCommand]
    private void RefrescarDatos()
    {
        CargarDatosDesdeMongo();
    }

    [RelayCommand]
    private void CrearMesa()
    {
        try
        {
            int numero;

            if (string.IsNullOrWhiteSpace(NumeroNuevaMesa))
                numero = ObtenerSiguienteNumeroMesa();
            else if (!int.TryParse(NumeroNuevaMesa, out numero) || numero <= 0)
            {
                Mensaje = "El número de mesa debe ser mayor que 0.";
                return;
            }

            if (_repoMesas.ObtenerTodas().Any(m => m.Numero == numero))
            {
                Mensaje = $"Ya existe una mesa con el número {numero}.";
                return;
            }

            int id = ObtenerSiguienteIdMesa();
            Mesa mesa = new Mesa(id, numero);

            _repoMesas.CrearMesa(mesa);
            NumeroNuevaMesa = "";
            CargarMesasDesdeMongo(id);
            Mensaje = $"Mesa {numero} añadida correctamente en MongoDB.";
        }
        catch (Exception ex)
        {
            Mensaje = "Error al crear mesa: " + ex.Message;
        }
    }

    [RelayCommand]
    private void ReservarMesa()
    {
        try
        {
            if (MesaSeleccionada is null)
            {
                Mensaje = "Selecciona una mesa.";
                return;
            }

            MesaSeleccionada.Reserva();
            _repoMesas.Actualizar(MesaSeleccionada);
            CargarMesasDesdeMongo(MesaSeleccionada.Id);
            Mensaje = $"Mesa {MesaSeleccionada.Numero} reservada.";
        }
        catch (Exception ex)
        {
            Mensaje = "Error al reservar mesa: " + ex.Message;
        }
    }

    [RelayCommand]
    private void AbrirComanda()
    {
        try
        {
            if (MesaSeleccionada is null)
            {
                Mensaje = "Selecciona una mesa.";
                return;
            }

            if (!MesaSeleccionada.EstaDisponible())
            {
                Mensaje = "La mesa no está libre. Libérala o cobra la comanda antes.";
                return;
            }

            int idComanda = ObtenerSiguienteIdComanda();
            _servicio.AbrirComanda(idComanda, MesaSeleccionada.Id);

            CargarMesasDesdeMongo(MesaSeleccionada.Id);
            Mensaje = $"Comanda {idComanda} abierta para la mesa {MesaSeleccionada?.Numero}.";
        }
        catch (Exception ex)
        {
            Mensaje = "Error al abrir comanda: " + ex.Message;
        }
    }

    [RelayCommand]
    private void CerrarMesa()
    {
        try
        {
            if (MesaSeleccionada is null)
            {
                Mensaje = "Selecciona una mesa.";
                return;
            }

            MesaSeleccionada.CerrarMesa();
            _repoMesas.Actualizar(MesaSeleccionada);
            CargarMesasDesdeMongo(MesaSeleccionada.Id);
            Mensaje = $"Mesa {MesaSeleccionada.Numero} liberada.";
        }
        catch (Exception ex)
        {
            Mensaje = "Error al liberar mesa: " + ex.Message;
        }
    }

    [RelayCommand]
    private void AgregarProductoCatalogo()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(NombreProducto))
            {
                Mensaje = "Escribe el nombre del producto.";
                return;
            }

            if (!decimal.TryParse(PrecioProducto.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out decimal precio) || precio <= 0)
            {
                Mensaje = "El precio debe ser mayor que 0. Ejemplo: 12,50";
                return;
            }

            string tipo = NormalizarTipoProducto(TipoProductoSeleccionado);

            Producto producto = tipo switch
            {
                "Bebida" => new Bebida(NombreProducto.Trim(), precio, ProductoTieneAlcohol),
                "Plato principal" => new PlatoPrincipal(NombreProducto.Trim(), precio, ProductoEsCaliente),
                "Postre" => new Postre(NombreProducto.Trim(), precio, ProductoTieneLactosa, ProductoEsCaliente),
                _ => new Bebida(NombreProducto.Trim(), precio, false)
            };

            _repoProductos.CrearProducto(producto);

            NombreProducto = "";
            PrecioProducto = "";
            ProductoTieneAlcohol = false;
            ProductoEsCaliente = false;
            ProductoTieneLactosa = false;
            TipoProductoSeleccionado = "Bebida";

            CargarProductosDesdeMongo();
            Mensaje = $"Producto añadido como {tipo}: {producto.Nombre}.";
        }
        catch (Exception ex)
        {
            Mensaje = "Error al añadir producto: " + ex.Message;
        }
    }

    [RelayCommand]
    private void AgregarProductoAComanda()
    {
        try
        {
            if (MesaSeleccionada is null)
            {
                Mensaje = "Selecciona una mesa.";
                return;
            }

            if (MesaSeleccionada.Comanda is null)
            {
                Mensaje = "La mesa no tiene comanda abierta.";
                return;
            }

            if (ProductoSeleccionadoParaComanda is null)
            {
                Mensaje = "Selecciona un producto del menú.";
                return;
            }

            if (!int.TryParse(CantidadProducto, out int cantidad) || cantidad <= 0)
            {
                Mensaje = "La cantidad debe ser mayor que 0.";
                return;
            }

            _servicio.AgregarProductoComanda(MesaSeleccionada.Comanda.Id, ProductoSeleccionadoParaComanda, cantidad);

            int idMesa = MesaSeleccionada.Id;
            string nombre = ProductoSeleccionadoParaComanda.Nombre;
            CargarMesasDesdeMongo(idMesa);
            Mensaje = $"Añadido a la comanda: {nombre} x{cantidad}.";
        }
        catch (Exception ex)
        {
            Mensaje = "Error al añadir producto a comanda: " + ex.Message;
        }
    }

    [RelayCommand]
    private void CobrarComanda()
    {
        try
        {
            if (MesaSeleccionada?.Comanda is null)
            {
                Mensaje = "No hay ninguna comanda para cobrar.";
                return;
            }

            int idMesa = MesaSeleccionada.Id;
            var ticket = _servicio.CobrarComanda(MesaSeleccionada.Comanda.Id);
            string resumenProductos = string.Join(", ", ticket.Productos);

            TicketsCaja.Insert(0, $"Mesa {ticket.NumeroMesa} | {ticket.Total:0.00} € | {resumenProductos}");

            CargarCierreCajaDesdeMongo();
            CargarMesasDesdeMongo(idMesa);
            Mensaje = $"Comanda cobrada. Total: {ticket.Total:0.00} €. Mesa liberada.";
        }
        catch (Exception ex)
        {
            Mensaje = "Error al cobrar comanda: " + ex.Message;
        }
    }

    private void CargarDatosDesdeMongo()
    {
        try
        {
            InicializarProductosSiHaceFalta();
            InicializarMesasSiHaceFalta();

            CargarProductosDesdeMongo();
            CargarMesasDesdeMongo(MesaSeleccionada?.Id);
            CargarCierreCajaDesdeMongo();

            Mensaje = "Datos cargados desde MongoDB correctamente.";
        }
        catch (Exception ex)
        {
            Mensaje = "No se ha podido cargar MongoDB: " + ex.Message;
        }
    }

    private void InicializarProductosSiHaceFalta()
    {
        if (_repoProductos.ObtenerTodos().Any())
            return;

        _repoProductos.CrearProducto(new Bebida("Coca-Cola", 2.50m, false));
        _repoProductos.CrearProducto(new Bebida("Agua", 1.50m, false));
        _repoProductos.CrearProducto(new Bebida("Cerveza artesanal", 4.00m, true));
        _repoProductos.CrearProducto(new PlatoPrincipal("Pizza", 12.00m, true));
        _repoProductos.CrearProducto(new PlatoPrincipal("Ensalada César", 9.00m, false));
        _repoProductos.CrearProducto(new PlatoPrincipal("Salmón a la plancha", 15.50m, true));
        _repoProductos.CrearProducto(new Postre("Flan casero", 4.50m, true, false));
        _repoProductos.CrearProducto(new Postre("Brownie caliente", 5.50m, true, true));
    }

    private void InicializarMesasSiHaceFalta()
    {
        if (_repoMesas.ObtenerTodas().Any())
            return;

        for (int i = 1; i <= 6; i++)
            _repoMesas.CrearMesa(new Mesa(i, i));
    }

    private void CargarProductosDesdeMongo()
    {
        var productosMongo = _repoProductos.ObtenerTodos()
            .OrderBy(p => p.Nombre)
            .ToList();

        Productos = new ObservableCollection<Producto>(productosMongo);
        AplicarFiltroProductos();

        ProductoSeleccionado = Productos.FirstOrDefault();
        ProductoSeleccionadoParaComanda = ProductosFiltrados.FirstOrDefault() ?? Productos.FirstOrDefault();
    }

    private void CargarMesasDesdeMongo(int? idSeleccionado)
    {
        var mesasMongo = _repoMesas.ObtenerTodas()
            .OrderBy(m => m.Numero)
            .ToList();

        Mesas = new ObservableCollection<Mesa>(mesasMongo);

        if (idSeleccionado is null)
            MesaSeleccionada = Mesas.FirstOrDefault();
        else
            MesaSeleccionada = Mesas.FirstOrDefault(m => m.Id == idSeleccionado) ?? Mesas.FirstOrDefault();

        OnPropertyChanged(nameof(TituloMesaSeleccionada));
        OnPropertyChanged(nameof(TotalComandaTexto));
        OnPropertyChanged(nameof(TiempoPreparacionTexto));
    }

    private void CargarCierreCajaDesdeMongo()
    {
        TotalCaja = _servicio.CajaTotal(DateTime.Now);
        CantidadVentas = _servicio.ObtenerCantidadVentas(DateTime.Now);
    }

    private void AplicarFiltroProductos()
    {
        var filtrados = Productos
            .Where(p => FiltroCategoria switch
            {
                "Bebidas" => p is Bebida,
                "Platos" => p is PlatoPrincipal,
                "Postres" => p is Postre,
                _ => true
            })
            .OrderBy(p => p.Precio)
            .ThenBy(p => p.Nombre)
            .ToList();

        ProductosFiltrados = new ObservableCollection<Producto>(filtrados);
        ProductoSeleccionadoParaComanda = ProductosFiltrados.FirstOrDefault() ?? Productos.FirstOrDefault();
    }

    private int ObtenerSiguienteIdMesa()
    {
        var mesasMongo = _repoMesas.ObtenerTodas();
        return mesasMongo.Count == 0 ? 1 : mesasMongo.Max(m => m.Id) + 1;
    }

    private int ObtenerSiguienteNumeroMesa()
    {
        var mesasMongo = _repoMesas.ObtenerTodas();
        return mesasMongo.Count == 0 ? 1 : mesasMongo.Max(m => m.Numero) + 1;
    }

    private int ObtenerSiguienteIdComanda()
    {
        var comandasMongo = _repoComandas.ObtenerTodas();
        return comandasMongo.Count == 0 ? 1 : comandasMongo.Max(c => c.Id) + 1;
    }

    private static string NormalizarTipoProducto(string? tipo)
    {
        tipo = tipo?.Trim() ?? "Bebida";

        if (tipo.Contains("Postre", StringComparison.OrdinalIgnoreCase))
            return "Postre";

        if (tipo.Contains("Plato", StringComparison.OrdinalIgnoreCase))
            return "Plato principal";

        return "Bebida";
    }
}
