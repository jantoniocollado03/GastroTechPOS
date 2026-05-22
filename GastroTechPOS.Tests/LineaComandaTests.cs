using GestorTechPOS.Models;

namespace GastroTechPOS.Tests;

public class LineaComandaTests
{
    [Fact]
    public void CrearLinea_ConCantidadValida_DeberiaGuardarDatos()
    {
        var producto = new Bebida("Agua", 1.50m, false);

        var linea = new LineaComanda(producto, 2);

        Assert.Equal(producto, linea.Producto);
        Assert.Equal(2, linea.Cantidad);
    }

    [Fact]
    public void CrearLinea_ConCantidadCero_DeberiaLanzarExcepcion()
    {
        var producto = new Bebida("Agua", 1.50m, false);

        Assert.Throws<ArgumentOutOfRangeException>(() => new LineaComanda(producto, 0));
    }

    [Fact]
    public void CrearLinea_ConCantidadNegativa_DeberiaLanzarExcepcion()
    {
        var producto = new Bebida("Agua", 1.50m, false);

        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new LineaComanda(producto, -1));
    }

    [Fact]
    public void CrearLinea_ConProductoNull_DeberiaLanzarExcepcion()
    {
        Assert.Throws<ArgumentNullException>(() =>
            new LineaComanda(null!, 1));
    }

    [Fact]
    public void CalcularSubtotal_DeberiaIncluirImpuestoYCantidad()
    {
        var producto = new Bebida("Cerveza", 10m, true);
        var linea = new LineaComanda(producto, 2);

        decimal subtotal = linea.CalcularSubtotal();

        Assert.Equal(24.20m, subtotal);
    }
}