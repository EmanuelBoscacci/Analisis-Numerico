using Analisis_Numerico_2026.Models.Unit1;

namespace Analisis_Numerico_2026.Services.Unit1
{
    public class RootFindingService
    {
        private readonly FunctionEvaluatorService _evaluador;
        private const int DecimalesRedondeo = 10;

        public RootFindingService(FunctionEvaluatorService evaluador)
        {
            _evaluador = evaluador;
        }

        private double Redondear(double valor)
        {
            if (double.IsNaN(valor) || double.IsInfinity(valor))
                return valor;
            return Math.Round(valor, DecimalesRedondeo, MidpointRounding.AwayFromZero);
        }

        // ╔═══════════════════════════════════════════════════════════╗
        // ║                    MÉTODO DE BISECCIÓN                    ║
        // ╚═══════════════════════════════════════════════════════════╝
        /// <summary>
        /// MÉTODO DE BISECCIÓN (Método cerrado)
        /// Calcula xr como el PUNTO MEDIO del intervalo: xr = (xi + xd) / 2
        /// </summary>
        public RootFindingResult Biseccion(RootFindingInput datosEntrada)
        {
            return MetodoCerrado(datosEntrada, "Bisección");
        }

        // ╔═══════════════════════════════════════════════════════════╗
        // ║                   MÉTODO DE REGLA FALSA                   ║
        // ╚═══════════════════════════════════════════════════════════╝
        /// <summary>
        /// MÉTODO DE REGLA FALSA (Método cerrado)
        /// Calcula xr trazando una RECTA entre los puntos (xi, f(xi)) y (xd, f(xd))
        /// y tomando donde esa recta cruza el eje X:
        ///   xr = (f(xd)*xi - f(xi)*xd) / (f(xd) - f(xi))
        /// </summary>
        public RootFindingResult ReglaFalsa(RootFindingInput datosEntrada)
        {
            return MetodoCerrado(datosEntrada, "Regla Falsa");
        }

        // ╔═══════════════════════════════════════════════════════════╗
        // ║              LÓGICA COMÚN DE MÉTODOS CERRADOS             ║
        // ╚═══════════════════════════════════════════════════════════╝
        /// <summary>
        /// Ambos métodos cerrados (Bisección y Regla Falsa) comparten la misma
        /// lógica. La ÚNICA diferencia es cómo calculan xr:
        ///   - Bisección:    xr = (xi + xd) / 2
        ///   - Regla Falsa:  xr = (f(xd)*xi - f(xi)*xd) / (f(xd) - f(xi))
        /// </summary>
        private RootFindingResult MetodoCerrado(RootFindingInput datosEntrada, string nombreMetodo)
        {
            var resultado = new RootFindingResult
            {
                Method = nombreMetodo,
                Function = datosEntrada.Function,
                Iterations = new List<RootIterationResult>()
            };

            // ═══════════════════════════════════════════════════════
            // PASO 1: Validar sintaxis
            // ═══════════════════════════════════════════════════════
            if (!_evaluador.InicializarFuncion(datosEntrada.Function))
            {
                resultado.Success = false;
                resultado.Message = "Error en la sintaxis de la función.";
                return resultado;
            }

            double limiteInferior = datosEntrada.Xi;
            double limiteSuperior = datosEntrada.Xd;
            double tolerancia = datosEntrada.Tolerance;
            int iteracionesMaximas = datosEntrada.MaxIterations;

            double funcionEnInferior = _evaluador.Evaluar(limiteInferior);
            double funcionEnSuperior = _evaluador.Evaluar(limiteSuperior);

            // ═══════════════════════════════════════════════════════
            // PASO 2: Verificar cambio de signo
            // ═══════════════════════════════════════════════════════
            if (funcionEnInferior * funcionEnSuperior > 0)
            {
                resultado.Success = false;
                resultado.Message = "La función no cambia de signo en el intervalo dado. Reingrese Xi y Xd.";
                return resultado;
            }

            // ═══════════════════════════════════════════════════════
            // PASO 3: Verificar si algún extremo ya es raíz
            // ═══════════════════════════════════════════════════════
            if (funcionEnInferior == 0)
            {
                resultado.Success = true;
                resultado.Message = "Xi es raíz exacta.";
                resultado.Root = limiteInferior;
                return resultado;
            }
            if (funcionEnSuperior == 0)
            {
                resultado.Success = true;
                resultado.Message = "Xd es raíz exacta.";
                resultado.Root = limiteSuperior;
                return resultado;
            }

            // ═══════════════════════════════════════════════════════
            // PASO 4: Bucle principal
            // ═══════════════════════════════════════════════════════
            double raizAnterior = 0;
            double raizAproximada = 0;
            double errorRelativo = 0;

            for (int iteracion = 1; iteracion <= iteracionesMaximas; iteracion++)
            {
                // ─── 4a: Calcular xr según el método ───
                //
                // BISECCIÓN:     Toma el punto medio
                //   xr = (xi + xd) / 2
                //
                // REGLA FALSA:   Traza una recta entre (xi, f(xi)) y (xd, f(xd))
                //                y calcula dónde esa recta corta el eje X
                //   xr = (f(xd) * xi - f(xi) * xd) / (f(xd) - f(xi))
                //
                if (nombreMetodo == "Bisección")
                {
                    raizAproximada = (limiteInferior + limiteSuperior) / 2.0;
                }
                else // Regla Falsa
                {
                    raizAproximada = (funcionEnSuperior * limiteInferior - funcionEnInferior * limiteSuperior)
                                   / (funcionEnSuperior - funcionEnInferior);
                }

                // ─── 4b: Evaluar la función en xr ───
                double funcionEnRaiz = _evaluador.Evaluar(raizAproximada);

                // ─── 4c: Calcular error relativo ───
                if (iteracion > 1)
                    errorRelativo = Math.Abs((raizAproximada - raizAnterior) / raizAproximada);

                // ─── 4d: Guardar iteración ───
                resultado.Iterations.Add(new RootIterationResult
                {
                    Iteration = iteracion,
                    Xi = Redondear(limiteInferior),
                    Xd = Redondear(limiteSuperior),
                    Xr = Redondear(raizAproximada),
                    FunctionValue = Redondear(funcionEnRaiz),
                    RelativeError = Redondear(iteracion == 1 ? 0 : errorRelativo),
                    IsRoot = Math.Abs(funcionEnRaiz) < tolerancia
                            || (iteracion > 1 && errorRelativo < tolerancia)
                });

                // ─── 4e: ¿Encontró la raíz? ───
                if (Math.Abs(funcionEnRaiz) < tolerancia
                    || (iteracion > 1 && errorRelativo < tolerancia))
                {
                    resultado.Success = true;
                    resultado.Message = "Raíz encontrada con la tolerancia indicada.";
                    resultado.Root = Redondear(raizAproximada);
                    resultado.IterationsUsed = iteracion;
                    resultado.FinalError = Redondear(errorRelativo);
                    return resultado;
                }

                // ─── 4f: Achicar el intervalo ───
                // (Igual en ambos métodos: nos quedamos con la mitad
                //  donde la función cambia de signo)
                if (funcionEnInferior * funcionEnRaiz < 0)
                {
                    limiteSuperior = raizAproximada;
                    funcionEnSuperior = funcionEnRaiz;
                }
                else
                {
                    limiteInferior = raizAproximada;
                    funcionEnInferior = funcionEnRaiz;
                }

                raizAnterior = raizAproximada;
            }

            // ═══════════════════════════════════════════════════════
            // PASO 5: No convergió
            // ═══════════════════════════════════════════════════════
            resultado.Success = false;
            resultado.Message = "No se encontró raíz en el número máximo de iteraciones.";
            resultado.Root = Redondear(raizAproximada);
            resultado.IterationsUsed = iteracionesMaximas;
            resultado.FinalError = Redondear(errorRelativo);
            return resultado;
        }

        // ╔═══════════════════════════════════════════════════════════╗
        // ║                 MÉTODO DE NEWTON-RAPHSON                  ║
        // ╚═══════════════════════════════════════════════════════════╝
        /// <summary>
        /// MÉTODO DE NEWTON-RAPHSON (Método abierto)
        /// 
        /// A diferencia de los métodos cerrados, NO necesita un intervalo [xi, xd].
        /// Solo necesita UN punto inicial (Xi) y usa la DERIVADA f'(x) para 
        /// calcular la siguiente aproximación:
        ///
        ///   xr = xi - f(xi) / f'(xi)
        ///
        /// Geométricamente: traza la recta TANGENTE a la curva en el punto (xi, f(xi))
        /// y toma donde esa tangente cruza el eje X como nueva aproximación.
        /// </summary>
        public RootFindingResult NewtonRaphson(RootFindingInput datosEntrada)
        {
            var resultado = new RootFindingResult
            {
                Method = "Newton-Raphson",
                Function = datosEntrada.Function,
                Iterations = new List<RootIterationResult>()
            };

            // ═══════════════════════════════════════════════════════
            // PASO 1: Validar sintaxis de la función
            // ═══════════════════════════════════════════════════════
            if (!_evaluador.InicializarFuncion(datosEntrada.Function))
            {
                resultado.Success = false;
                resultado.Message = "Error en la sintaxis de la función.";
                return resultado;
            }

            double puntoActual = datosEntrada.Xi;        // xi (punto donde estamos parados)
            double tolerancia = datosEntrada.Tolerance;
            int iteracionesMaximas = datosEntrada.MaxIterations;

            double raizAproximada = 0;                    // xr (nueva aproximación)
            double errorRelativo = 0;

            // ═══════════════════════════════════════════════════════
            // PASO 2: Bucle principal
            // ═══════════════════════════════════════════════════════
            for (int iteracion = 1; iteracion <= iteracionesMaximas; iteracion++)
            {
                // ─── 2a: Evaluar f(xi) y f'(xi) ───
                double funcionEnPunto = _evaluador.Evaluar(puntoActual);
                double derivadaEnPunto = _evaluador.CalcularDerivada(puntoActual);

                // ─── 2b: Verificar que la derivada no sea cero ───
                // Si f'(xi) = 0, la tangente es horizontal y no cruza el eje X.
                // El método no puede continuar.
                if (Math.Abs(derivadaEnPunto) < 1e-15)
                {
                    resultado.Success = false;
                    resultado.Message = $"La derivada es cero en x = {puntoActual}. El método no puede continuar.";
                    resultado.Root = Redondear(puntoActual);
                    resultado.IterationsUsed = iteracion;
                    resultado.FinalError = Redondear(errorRelativo);
                    return resultado;
                }

                // ─── 2c: Calcular la nueva aproximación ───
                //
                // FÓRMULA DE NEWTON-RAPHSON:
                //   xr = xi - f(xi) / f'(xi)
                //
                // Es decir: al punto actual le restamos la corrección
                // que nos da la tangente.
                //
                raizAproximada = puntoActual - (funcionEnPunto / derivadaEnPunto);

                // ─── 2d: Calcular error relativo ───
                if (raizAproximada != 0)
                    errorRelativo = Math.Abs((raizAproximada - puntoActual) / raizAproximada);

                // ─── 2e: Evaluar f(xr) para mostrar en la tabla ───
                double funcionEnRaiz = _evaluador.Evaluar(raizAproximada);

                // ─── 2f: Guardar iteración ───
                // En Newton-Raphson:
                //   Xi  = punto actual (de donde partimos)
                //   Xd  = f'(xi) (la derivada, para referencia en la tabla)
                //   Xr  = nueva aproximación calculada
                resultado.Iterations.Add(new RootIterationResult
                {
                    Iteration = iteracion,
                    Xi = Redondear(puntoActual),
                    Xd = Redondear(derivadaEnPunto),
                    Xr = Redondear(raizAproximada),
                    FunctionValue = Redondear(funcionEnRaiz),
                    RelativeError = Redondear(errorRelativo),
                    IsRoot = Math.Abs(funcionEnRaiz) < tolerancia || errorRelativo < tolerancia
                });

                // ─── 2g: ¿Encontró la raíz? ───
                if (Math.Abs(funcionEnRaiz) < tolerancia || errorRelativo < tolerancia)
                {
                    resultado.Success = true;
                    resultado.Message = "Raíz encontrada con la tolerancia indicada.";
                    resultado.Root = Redondear(raizAproximada);
                    resultado.IterationsUsed = iteracion;
                    resultado.FinalError = Redondear(errorRelativo);
                    return resultado;
                }

                // ─── 2h: Avanzar al siguiente punto ───
                // El xr de esta iteración se convierte en el xi de la siguiente.
                puntoActual = raizAproximada;
            }

            // ═══════════════════════════════════════════════════════
            // PASO 3: No convergió
            // ═══════════════════════════════════════════════════════
            resultado.Success = false;
            resultado.Message = "No se encontró raíz en el número máximo de iteraciones.";
            resultado.Root = Redondear(raizAproximada);
            resultado.IterationsUsed = iteracionesMaximas;
            resultado.FinalError = Redondear(errorRelativo);
            return resultado;
        }

        // ╔═══════════════════════════════════════════════════════════╗
        // ║                   MÉTODO DE LA SECANTE                    ║
        // ╚═══════════════════════════════════════════════════════════╝
        /// <summary>
        /// MÉTODO DE LA SECANTE (Método abierto)
        /// 
        /// Es como Newton-Raphson pero SIN DERIVADA.
        /// En vez de usar f'(xi), la APROXIMA con una recta secante
        /// que pasa por los dos últimos puntos:
        ///
        ///   f'(xi) ≈ (f(xi) - f(xi_anterior)) / (xi - xi_anterior)
        ///
        /// Reemplazando en la fórmula de Newton:
        ///   xr = xi - f(xi) * (xi - xi_anterior) / (f(xi) - f(xi_anterior))
        ///
        /// Necesita DOS puntos iniciales: Xi (punto anterior) y Xd (punto actual).
        /// NO requiere que haya cambio de signo entre ellos.
        /// </summary>
        public RootFindingResult Secante(RootFindingInput datosEntrada)
        {
            var resultado = new RootFindingResult
            {
                Method = "Secante",
                Function = datosEntrada.Function,
                Iterations = new List<RootIterationResult>()
            };

            // ═══════════════════════════════════════════════════════
            // PASO 1: Validar sintaxis de la función
            // ═══════════════════════════════════════════════════════
            if (!_evaluador.InicializarFuncion(datosEntrada.Function))
            {
                resultado.Success = false;
                resultado.Message = "Error en la sintaxis de la función.";
                return resultado;
            }

            // Secante necesita DOS puntos iniciales:
            //   puntoAnterior = x₀ (el más viejo)  → viene de Xi
            //   puntoActual   = x₁ (el más nuevo)  → viene de Xd
            double puntoAnterior = datosEntrada.Xi;
            double puntoActual = datosEntrada.Xd;
            double tolerancia = datosEntrada.Tolerance;
            int iteracionesMaximas = datosEntrada.MaxIterations;

            double raizAproximada = 0;
            double errorRelativo = 0;

            // ═══════════════════════════════════════════════════════
            // PASO 2: Bucle principal
            // ═══════════════════════════════════════════════════════
            for (int iteracion = 1; iteracion <= iteracionesMaximas; iteracion++)
            {
                // ─── 2a: Evaluar f en ambos puntos ───
                double funcionEnAnterior = _evaluador.Evaluar(puntoAnterior);
                double funcionEnActual = _evaluador.Evaluar(puntoActual);

                // ─── 2b: Verificar que f(xi) ≠ f(xi_anterior) ───
                // Si son iguales, la secante es horizontal → no cruza el eje X
                // (misma situación que f'(x)=0 en Newton-Raphson)
                double diferenciaFunciones = funcionEnActual - funcionEnAnterior;

                if (Math.Abs(diferenciaFunciones) < 1e-15)
                {
                    resultado.Success = false;
                    resultado.Message = $"f(xi) y f(xi anterior) son iguales. La secante es horizontal. El método no puede continuar.";
                    resultado.Root = Redondear(puntoActual);
                    resultado.IterationsUsed = iteracion;
                    resultado.FinalError = Redondear(errorRelativo);
                    return resultado;
                }

                // ─── 2c: Calcular la nueva aproximación ───
                //
                // FÓRMULA DE LA SECANTE:
                //   xr = xi - f(xi) * (xi - xi_anterior) / (f(xi) - f(xi_anterior))
                //
                // Comparación con Newton-Raphson:
                //   Newton:   xr = xi - f(xi) / f'(xi)            ← derivada EXACTA
                //   Secante:  xr = xi - f(xi) / [(f(xi)-f(x₀))/(xi-x₀)]  ← derivada APROXIMADA
                //
                // Es decir: donde Newton usa f'(xi), Secante usa la pendiente
                // de la recta que une los dos últimos puntos.
                //
                raizAproximada = puntoActual - funcionEnActual * (puntoActual - puntoAnterior)
                                                               / diferenciaFunciones;

                // ─── 2d: Calcular error relativo ───
                if (raizAproximada != 0)
                    errorRelativo = Math.Abs((raizAproximada - puntoActual) / raizAproximada);

                // ─── 2e: Evaluar f(xr) para mostrar en la tabla ───
                double funcionEnRaiz = _evaluador.Evaluar(raizAproximada);

                // ─── 2f: Guardar iteración ───
                // En Secante:
                //   Xi = punto anterior (x₀)
                //   Xd = punto actual (x₁)
                //   Xr = nueva aproximación calculada
                resultado.Iterations.Add(new RootIterationResult
                {
                    Iteration = iteracion,
                    Xi = Redondear(puntoAnterior),
                    Xd = Redondear(puntoActual),
                    Xr = Redondear(raizAproximada),
                    FunctionValue = Redondear(funcionEnRaiz),
                    RelativeError = Redondear(errorRelativo),
                    IsRoot = Math.Abs(funcionEnRaiz) < tolerancia || errorRelativo < tolerancia
                });

                // ─── 2g: ¿Encontró la raíz? ───
                if (Math.Abs(funcionEnRaiz) < tolerancia || errorRelativo < tolerancia)
                {
                    resultado.Success = true;
                    resultado.Message = "Raíz encontrada con la tolerancia indicada.";
                    resultado.Root = Redondear(raizAproximada);
                    resultado.IterationsUsed = iteracion;
                    resultado.FinalError = Redondear(errorRelativo);
                    return resultado;
                }

                // ─── 2h: Avanzar los puntos ───
                // Los dos puntos se "corren" hacia adelante:
                //   x₀ viejo → se descarta
                //   x₁ viejo → se convierte en x₀ nuevo
                //   xr       → se convierte en x₁ nuevo
                puntoAnterior = puntoActual;
                puntoActual = raizAproximada;
            }

            // ═══════════════════════════════════════════════════════
            // PASO 3: No convergió
            // ═══════════════════════════════════════════════════════
            resultado.Success = false;
            resultado.Message = "No se encontró raíz en el número máximo de iteraciones.";
            resultado.Root = Redondear(raizAproximada);
            resultado.IterationsUsed = iteracionesMaximas;
            resultado.FinalError = Redondear(errorRelativo);
            return resultado;
        }
    }
}