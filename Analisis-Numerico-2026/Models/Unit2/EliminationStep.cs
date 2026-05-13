namespace Analisis_Numerico_2026.Models.Unit2
{
    /// <summary>
    /// Representa el estado de la matriz aumentada después de
    /// procesar cada fila pivote (diagonal principal).
    /// Se usa para mostrar la tabla de pasos en la UI.
    /// </summary>
    public class EliminationStep
    {
        // Número de paso (1, 2, 3...)
        public int Paso { get; set; }

        // Descripción de qué operación se hizo en este paso
        public string Descripcion { get; set; } = string.Empty;

        // Estado de la matriz aumentada después de este paso.
        // Es una copia de la matriz en ese momento.
        // Filas x (Dimension+1) columnas
        public List<List<double>> Matriz { get; set; } = new();
    }
}
