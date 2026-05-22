using GestorTechPOS.Models;

namespace GastroTechPOS.Tests;

public class ProductoTests
{
    [Fact]
    public void CrearBebida_ConDatosValidos_DeberiaGuardarDatos()
    {
        var bebida = new Bebida("Coca Cola", 2.50m, false);

        Assert.Equal("Coca Cola", bebida.Nombre);
        Assert.Equal(2.50m, bebida.Precio);
        Assert.False(bebida.TieneAlcohol);
    }

    [Fact]
    public void CrearPlatoPrincipal_ConDatosValidos_DeberiaGuardarDatos()
    {
        var plato = new PlatoPrincipal("Pizza", 12.50m, true);

        Assert.Equal("Pizza", plato.Nombre);
        Assert.Equal(12.50m, plato.Precio);
        Assert.True(plato.EsCaliente);
    }

    [Fact]
    public void CrearPostre_ConDatosValidos_DeberiaGuardarDatos()
    {
        var postre = new Postre("Tarta", 4.50m, true, false);

        Assert.Equal("Tarta", postre.Nombre);
        Assert.Equal(4.50m, postre.Precio);
        Assert.True(postre.TieneLactosa);
        Assert.False(postre.EsCaliente);
    }

    [Fact]
    public void CrearProducto_ConNombreVacio_DeberiaLanzarExcepcion()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new Bebida("", 2.50m, false));
    }

    [Fact]
    public void CrearProducto_ConPrecioCero_DeberiaLanzarExcepcion()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Bebida("Agua", 0m, false));
    }

    [Fact]
    public void CrearProducto_ConPrecioNegativo_DeberiaLanzarExcepcion()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new Bebida("Coca Cola", -1m, false));
    }

    [Fact]
    public void ToString_DeberiaContenerNombreYTipo()
    {
        var plato = new PlatoPrincipal("Pizza", 12.50m, true);

        string texto = plato.ToString();

        Assert.Contains("Pizza", texto);
        Assert.Contains("plato", texto);
    }
}