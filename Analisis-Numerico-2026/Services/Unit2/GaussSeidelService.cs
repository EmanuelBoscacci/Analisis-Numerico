using Analisis_Numerico_2026.Models.Unit2;

namespace Analisis_Numerico_2026.Services.Unit2
{
    public class GaussSeidelService
    {
        // ── Constantes fijas del algoritmo (según pseudocódigo) ──
        private const int DecimalesSolucion = 4;

        private double Redondear(double valor, int decimales = DecimalesSolucion)
        {
            if (double.IsNaN(valor) || double.IsInfinity(valor))
                return valor;
            return Math.Round(valor, decimales, MidpointRounding.AwayFromZero);
        }

        // ╔═══════════════════════════════════════════════════════════╗
        // ║                    GAUSS-SEIDEL                           ║
        // ╚═══════════════════════════════════════════════════════════╝
        public LinearSystemResult Resolver(LinearSystemInput datosEntrada)
        {
            var resultado = new LinearSystemResult
            {
                Method = "Gauss-Seidel",
                Dimension = datosEntrada.Dimension,
                Solucion = new List<double>(),
                Pasos = new List<EliminationStep>(),
                IteracionesSeidel = new List<SeidelIterationStep>()
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
                    matriz[fila][col] = datosEntrada.MatrizAumentada[fila * (dimension + 1) + col];
            }

            // ═══════════════════════════════════════════════════════
            // PASO 3: Verificar que ningún coeficiente diagonal sea 0
            // ═══════════════════════════════════════════════════════
            for (int i = 0; i < dimension; i++)
            {
                if (Math.Abs(matriz[i][i]) < 1e-15)
                {
                    resultado.Success = false;
                    resultado.Message = $"El coeficiente diagonal en la fila {i + 1} es cero. " +
                                        "La matriz debe ser diagonalmente dominante (o reordenar las ecuaciones).";
                    return resultado;
                }
            }

            // ═══════════════════════════════════════════════════════
            // PASO 3.5: Verificar matriz diagonalmente dominante
            // ═══════════════════════════════════════════════════════
            bool esDiagonalmenteDominante = true;
            for (int i = 0; i < dimension; i++)
            {
                double sumaFila = 0;
                for (int j = 0; j < dimension; j++)
                {
                    if (i != j) sumaFila += Math.Abs(matriz[i][j]);
                }
                
                if (Math.Abs(matriz[i][i]) <= sumaFila)
                {
                    esDiagonalmenteDominante = false;
                    break;
                }
            }

            string warningMessage = "";
            if (!esDiagonalmenteDominante)
            {
                warningMessage = "Advertencia: La matriz no es estrictamente diagonal dominante. La convergencia no está garantizada. ";
            }

            // ═══════════════════════════════════════════════════════
            // PASO 4: Algoritmo Gauss-Seidel (fiel al pseudocódigo)
            // ═══════════════════════════════════════════════════════
            double[] vectorResultado = new double[dimension]; // Initialize() → todos en cero
            double[] vectorAnterior = new double[dimension];
            bool esSolucion = false;
            int contador = 0;

            while (contador <= datosEntrada.MaxIteraciones && !esSolucion)
            {
                contador++;

                // Si (contador > 1) → guardar iteración anterior
                if (contador > 1)
                    vectorResultado.CopyTo(vectorAnterior, 0);

                // Para cada fila: calcular nuevo valor de la incógnita
                for (int row = 0; row < dimension; row++)
                {
                    double resultado_row = matriz[row][dimension]; // término independiente b
                    double coeficienteIncognita = matriz[row][row];     // coeficiente diagonal

                    for (int col = 0; col < dimension; col++)
                    {
                        if (row != col)
                            resultado_row -= matriz[row][col] * vectorResultado[col];
                        // Gauss-Seidel: vectorResultado[col] ya tiene el valor
                        // actualizado de esta misma iteración si col < row
                    }

                    // coeficienteIncognita = resultado / coeficienteIncognita
                    vectorResultado[row] = resultado_row / coeficienteIncognita;
                }

                // ── Guardar snapshot de esta iteración ──
                var paso = new SeidelIterationStep
                {
                    Iteracion = contador,
                    Vector = vectorResultado.Select(v => Redondear(v)).ToList()
                };

                // Calcular errores y verificar convergencia (desde iteración 2)
                int contadorMismoResultado = 0;

                if (contador > 1)
                {
                    for (int i = 0; i < dimension; i++)
                    {
                        double errorRelativo = Math.Abs(
                            vectorResultado[i] != 0
                                ? (vectorResultado[i] - vectorAnterior[i]) / vectorResultado[i]
                                : vectorResultado[i] - vectorAnterior[i]);

                        paso.Errores.Add(Redondear(errorRelativo, 6));

                        if (errorRelativo < datosEntrada.Tolerancia)
                            contadorMismoResultado++;
                    }

                    // esSolucion = contadorMismoResultado == dimension
                    esSolucion = contadorMismoResultado == dimension;
                }

                resultado.IteracionesSeidel.Add(paso);
            }

            // ═══════════════════════════════════════════════════════
            // PASO 5: Retorno
            // ═══════════════════════════════════════════════════════
            if (contador <= datosEntrada.MaxIteraciones)
            {
                resultado.Success = true;
                resultado.Message = warningMessage + $"Solución encontrada en {contador} iteración/es.";
                resultado.Solucion = vectorResultado.Select(v => Redondear(v)).ToList();
            }
            else
            {
                resultado.Success = false;
                resultado.Message = warningMessage + $"El método no convergió en {datosEntrada.MaxIteraciones} iteraciones. " +
                                    "Verificá que la matriz sea diagonalmente dominante.";
            }

            return resultado;
        }
    }
}