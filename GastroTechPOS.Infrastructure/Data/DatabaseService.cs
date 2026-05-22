using GestorTechPOS.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace GastroTechPOS.Infrastructure.Data;

public sealed class DatabaseService
{
    private static DatabaseService? _instancia;

    public MongoClient Cliente { get; }
    public IMongoDatabase Database { get; }

    private DatabaseService()
    {
        RegistrarConfiguracionBson();

        const string connectionString = "mongodb+srv://jantoniocollado03:JbMj_3112003@gastrotechpos.5xscg4i.mongodb.net/?appName=GastroTechPOS";

        Cliente = new MongoClient(connectionString);
        Database = Cliente.GetDatabase("GastroTechPOS");
    }

    public static DatabaseService Instancia
    {
        get
        {
            if (_instancia == null)
                _instancia = new DatabaseService();

            return _instancia;
        }
    }

    private static void RegistrarConfiguracionBson()
    {
        try
        {
            BsonSerializer.RegisterSerializer(typeof(decimal), new DecimalSerializer(BsonType.Decimal128));
        }
        catch
        {
            // El serializador ya estaba registrado.
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Producto)))
        {
            BsonClassMap.RegisterClassMap<Producto>(mapa =>
            {
                mapa.AutoMap();
                mapa.SetIsRootClass(true);
                mapa.SetDiscriminator("Producto");
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Bebida)))
        {
            BsonClassMap.RegisterClassMap<Bebida>(mapa =>
            {
                mapa.AutoMap();
                mapa.SetDiscriminator("Bebida");
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(PlatoPrincipal)))
        {
            BsonClassMap.RegisterClassMap<PlatoPrincipal>(mapa =>
            {
                mapa.AutoMap();
                mapa.SetDiscriminator("PlatoPrincipal");
            });
        }

        if (!BsonClassMap.IsClassMapRegistered(typeof(Postre)))
        {
            BsonClassMap.RegisterClassMap<Postre>(mapa =>
            {
                mapa.AutoMap();
                mapa.SetDiscriminator("Postre");
            });
        }
    }
}
