using System;
using System.Globalization;
using GestorTechPOS.Models;
using GastroTechPOS.Infrastructure.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace GastroTechPOS.Infrastructure.Repositories;

public class RepositorioProductosMongo
{
    private readonly IMongoCollection<BsonDocument> _productos;

    public RepositorioProductosMongo()
    {
        _productos = DatabaseService.Instancia.Database.GetCollection<BsonDocument>("productos");
    }

    public void CrearProducto(Producto producto)
    {
        BsonDocument documento = ConvertirADocumento(producto);
        _productos.InsertOne(documento);
    }

    public List<Producto> ObtenerTodos()
    {
        return _productos
            .Find(_ => true)
            .ToList()
            .Select(ConvertirAProducto)
            .ToList();
    }

    public bool ExisteProducto(string nombre)
    {
        return _productos
            .Find(p => p["Nombre"] == nombre)
            .Any();
    }

    private static BsonDocument ConvertirADocumento(Producto producto)
    {
        string tipo = producto switch
        {
            Bebida => "Bebida",
            PlatoPrincipal => "Plato principal",
            Postre => "Postre",
            _ => "Producto"
        };

        bool tieneAlcohol = producto is Bebida bebida && bebida.TieneAlcohol;
        bool esCaliente = producto switch
        {
            PlatoPrincipal plato => plato.EsCaliente,
            Postre postre => postre.EsCaliente,
            _ => false
        };
        bool tieneLactosa = producto is Postre postreConLactosa && postreConLactosa.TieneLactosa;

        return new BsonDocument
        {
            { "Id", Random.Shared.Next(1, int.MaxValue) },
            { "Nombre", producto.Nombre },
            { "Precio", new BsonDecimal128(Decimal128.Parse(producto.Precio.ToString(CultureInfo.InvariantCulture))) },
            { "Tipo", tipo },
            { "TieneAlcohol", tieneAlcohol },
            { "EsCaliente", esCaliente },
            { "TieneLactosa", tieneLactosa }
        };
    }

    private static Producto ConvertirAProducto(BsonDocument documento)
    {
        string nombre = documento.GetValue("Nombre", "Producto").AsString;
        decimal precio = ObtenerDecimal(documento.GetValue("Precio", new BsonDecimal128(Decimal128.Parse("1"))));
        string tipo = documento.GetValue("Tipo", "Bebida").AsString;
        bool tieneAlcohol = documento.GetValue("TieneAlcohol", false).AsBoolean;
        bool esCaliente = documento.GetValue("EsCaliente", false).AsBoolean;
        bool tieneLactosa = documento.GetValue("TieneLactosa", false).AsBoolean;

        return tipo switch
        {
            "Bebida" => new Bebida(nombre, precio, tieneAlcohol),
            "Plato" => new PlatoPrincipal(nombre, precio, esCaliente),
            "Plato principal" => new PlatoPrincipal(nombre, precio, esCaliente),
            "Postre" => new Postre(nombre, precio, tieneLactosa, esCaliente),
            _ => new Bebida(nombre, precio, false)
        };
    }

    private static decimal ObtenerDecimal(BsonValue valor)
    {
        return valor.BsonType switch
        {
            BsonType.Decimal128 => Decimal128.ToDecimal(valor.AsDecimal128),
            BsonType.Double => Convert.ToDecimal(valor.AsDouble, CultureInfo.InvariantCulture),
            BsonType.Int32 => valor.AsInt32,
            BsonType.Int64 => valor.AsInt64,
            BsonType.String => decimal.Parse(valor.AsString.Replace(',', '.'), CultureInfo.InvariantCulture),
            _ => 1m
        };
    }
}
