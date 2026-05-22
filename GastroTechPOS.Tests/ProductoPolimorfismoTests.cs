using GestorTechPOS.Models;

namespace GastroTechPOS.Tests;

public class ProductoPolimorfismoTests
{
    [Fact]
    public void BebidaConAlcohol_DeberiaCalcularImpuesto21PorCiento()
    {
        var bebida = new Bebida("Cerveza", 10m, true);

        Assert.Equal(2.10m, bebida.CalcularImpuesto());
    }

    [Fact]
    public void BebidaSinAlcohol_DeberiaCalcularImpuesto10PorCiento()
    {
        var bebida = new Bebida("Agua", 10m, false);

        Assert.Equal(1.00m, bebida.CalcularImpuesto());
    }

    [Fact]
    public void PlatoPrincipal_DeberiaCalcularImpuesto21PorCiento()
    {
        var plato = new PlatoPrincipal("Pizza", 20m, true);

        Assert.Equal(4.20m, plato.CalcularImpuesto());
    }

    [Fact]
    public void PlatoCaliente_DeberiaTardar20Minutos()
    {
        var plato = new PlatoPrincipal("Pizza", 12m, true);

        Assert.Equal(20, plato.TiempoPreparacion());
    }

    [Fact]
    public void PlatoFrio_DeberiaTardar8Minutos()
    {
        var plato = new PlatoPrincipal("Ensalada", 8m, false);

        Assert.Equal(8, plato.TiempoPreparacion());
    }

    [Fact]
    public void PostreCaliente_DeberiaTardar12Minutos()
    {
        var postre = new Postre("Brownie", 5m, true, true);

        Assert.Equal(12, postre.TiempoPreparacion());
    }

    [Fact]
    public void PostreFrio_DeberiaTardar3Minutos()
    {
        var postre = new Postre("Helado", 5m, true, false);

        Assert.Equal(3, postre.TiempoPreparacion());
    }
}