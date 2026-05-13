using Analisis_Numerico_2026.Models.Unit2;

namespace Analisis_Numerico_2026.Services.Unit2
{
    public class GaussJordanService
    {
        // Decimales para el vector solución final (presentación limpia)
        private const int DecimalesSolucion = 4;

        private double Redondear(double valor, int decimales = DecimalesSolucion)
        {
            if (double.IsNaN(valor) || double.IsInfinity(valor))
                return valor;
            return Math.Round(valor, decimales, MidpointRounding.AwayFromZero);
        }

        // ╔═══════════════════════════════════════════════════════════╗
        // ║                    GAUSS-JORDAN                           ║
        // ╚═══════════════════════════════════════════════════════════╝
        public LinearSystemResult Resolver(LinearSystemInput datosEntrada)
        {
            var resultado = new LinearSystemResult
            {
                Method = "Gauss-Jordan",
                Dimension = datosEntrada.Dimension,
                Pasos = new List<EliminationStep>(),
                Solucion = new List<double>()
            };

            int dimension = datosEntrada.Dimension;

            // ═══════════════════════════════════════════════════════
            // PASO 1: Validaciones básicas
            // ═══════════════════════════════════════════════════════
            if (dimension < 2)
            {
                resultado.Success = false;
                resultado.Message = "La dimensión debe ser al menos 2.";
                return resultado;
            }

            int elementosEsperados = dimension * (dimension + 1);
            if (datosEntrada.MatrizAumentada.Count != elementosEsperados)
            {
                resultado.Success = false;
                resultado.Message = $"La matriz aumentada debe tener {elementosEsperados} elementos para dimensión {dimension}.";
                return resultado;
            }

            // ═══════════════════════════════════════════════════════
            // PASO 2: Convertir la lista aplanada en matriz 2D
            // ═══════════════════════════════════════════════════════
            double[][] matriz = new double[dimension][];
            for (int fila = 0; fila < dimension; fila++)
            {
                matriz[fila] = new double[dimension + 1];
                for (int col = 0; col <= dimension; col++)
                {
                    matriz[fila][col] = datosEntrada.MatrizAumentada[fila * (dimension + 1) + col];
                }
            }

            // ═══════════════════════════════════════════════════════
            // PASO 3: Algoritmo Gauss-Jordan
            // ═══════════════════════════════════════════════════════
            for (int rowDiag = 0; rowDiag < dimension; rowDiag++)
            {
                // ─── 3.0: Pivoteo Parcial ───
                int maxRow = rowDiag;
                double maxVal = Math.Abs(matriz[rowDiag][rowDiag]);
                
                for (int i = rowDiag + 1; i < dimension; i++)
                {
                    if (Math.Abs(matriz[i][rowDiag]) > maxVal)
                    {
                        maxVal = Math.Abs(matriz[i][rowDiag]);
                        maxRow = i;
                    }
                }

                if (maxRow != rowDiag)
                {
                    // Intercambiar la fila actual con la fila del pivote máximo
                    double[] temp = matriz[rowDiag];
                    matriz[rowDiag] = matriz[maxRow];
                    matriz[maxRow] = temp;
                }

                // ─── 3a: Obtener el pivote (coeficiente diagonal) ───
                double pivote = matriz[rowDiag][rowDiag];

                // ─── 3b: Verificar que el pivote no sea cero ───
                if (Math.Abs(pivote) < 1e-15)
                {
                    resultado.Success = false;
                    resultado.Message = $"El pivote en la columna {rowDiag + 1} es cero y no se pudo encontrar un pivote válido mediante intercambio. " +
                                        $"El sistema no tiene solución única.";
                    return resultado;
                }

                // ─── 3c: Dividir toda la fila rowDiag por el pivote ───
                // Objetivo: que el coeficiente diagonal quede = 1
                for (int col = 0; col <= dimension; col++)
                {
                    matriz[rowDiag][col] = matriz[rowDiag][col] / pivote;
                }

                // ─── 3d: Eliminar en TODAS las otras filas ───
                for (int row = 0; row < dimension; row++)
                {
                    if (row == rowDiag)
                        continue;

                    double coeficienteCero = matriz[row][rowDiag];

                    for (int col = 0; col <= dimension; col++)
                    {
                        matriz[row][col] = matriz[row][col] - coeficienteCero * matriz[rowDiag][col];
                    }
                }

                // ─── 3e: Guardar estado de la matriz en este paso ───
                resultado.Pasos.Add(new EliminationStep
                {
                    Paso = rowDiag + 1,
                    Descripcion = $"Paso {rowDiag + 1}: fila {rowDiag + 1} normalizada y columna {rowDiag + 1} eliminada",
                    Matriz = CopiarMatriz(matriz, dimension)
                });
            }

            // ═══════════════════════════════════════════════════════
            // PASO 4: Extraer el vector solución
            // ═══════════════════════════════════════════════════════
            // Redondeamos con DecimalesSolucion para evitar artefactos
            // de punto flotante como 1.7999999999 en lugar de 1.8
            for (int fila = 0; fila < dimension; fila++)
            {
                resultado.Solucion.Add(Redondear(matriz[fila][dimension], DecimalesSolucion));
            }

            resultado.Success = true;
            resultado.Message = "Sistema resuelto correctamente.";
            return resultado;
        }

        // ─── Utilidad: copiar estado actual de la matriz ───
        private List<List<double>> CopiarMatriz(double[][] matriz, int dimension)
        {
            var copia = new List<List<double>>();
            for (int fila = 0; fila < dimension; fila++)
            {
                var filaCopia = new List<double>();
                for (int col = 0; col <= dimension; col++)
                    filaCopia.Add(Redondear(matriz[fila][col])); // Redondear para mostrar en UI
                copia.Add(filaCopia);
            }
            return copia;
        }
    }
}