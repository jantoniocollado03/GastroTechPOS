namespace GestorTechPOS.DTOs;

public record TicketDto(
    int IdComanda,
    int NumeroMesa,
    List<string> Productos,
    decimal Total,
    DateTime FechaPago
    );