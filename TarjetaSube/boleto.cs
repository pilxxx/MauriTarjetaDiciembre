using System;

namespace TarjetaSube
{
    public class Boleto
    {
        public string LineaColectivo { get; }
        public decimal ImportePagado { get; }
        public decimal SaldoRestante { get; }
        public DateTime FechaHora { get; }
        public bool EsTransbordo { get; }
        public int IdTarjeta { get; }
        public string TipoTarjeta { get; }

        public Boleto(string linea, decimal importe, decimal saldo, int idTarjeta, string tipoTarjeta, bool esTransbordo = false, DateTime? fechaHora = null)
        {
            LineaColectivo = linea;
            ImportePagado = importe;
            SaldoRestante = saldo;
            FechaHora = fechaHora ?? DateTime.Now;
            EsTransbordo = esTransbordo;
            IdTarjeta = idTarjeta;
            TipoTarjeta = tipoTarjeta;
        }

        public void MostrarInformacion()
        {
            Console.WriteLine("================================");
            Console.WriteLine("    BOLETO DE COLECTIVO    ");
            Console.WriteLine("================================");
            Console.WriteLine($"Línea: {LineaColectivo}");
            Console.WriteLine($"Fecha: {FechaHora:dd/MM/yyyy}");
            Console.WriteLine($"Hora: {FechaHora:HH:mm:ss}");
            Console.WriteLine($"Tipo: {TipoTarjeta}");
            Console.WriteLine($"ID Tarjeta: {IdTarjeta}");
            Console.WriteLine($"Importe pagado: ${ImportePagado}");
            Console.WriteLine($"Saldo restante: ${SaldoRestante}");
            if (EsTransbordo) Console.WriteLine("*** TRASBORDO ***");
            Console.WriteLine("================================");
        }
    }
}