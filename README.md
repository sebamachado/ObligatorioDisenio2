# BiosMessenger - Obligatorio Diseño 2

Esta solución replica la arquitectura indicada en el caso de estudio de Entity Framework Database First y cumple con los requerimientos del obligatorio.

## Estructura

- `ObligatorioDisenio2.sln`: solución con dos proyectos, el sitio WebForms (`Sitio`) y la biblioteca de acceso a datos (`ModeloEF`).
- `ModeloEF`: biblioteca .NET Framework 4.7.2 con Entity Framework 6 Database First.
  - `Entities`: clases parciales generadas a partir del esquema `BiosMessenger`.
  - `Services`: lógica de negocio y acceso a datos. Solo se consumen los procedimientos almacenados permitidos (`spMensaje_Alta`, `spMensaje_AddDestinatario`, `spUsuario_Baja`).
  - `Validador`: centraliza todas las validaciones de negocio (usuarios, mensajes, destinatarios, fechas, formato de credenciales, etc.).
- `Sitio`: Web Site ASP.NET WebForms que replica la estructura del caso de estudio (master, Default, ABMUsuarios, AltaMensaje y Bandeja).

## Base de datos

1. Instale SQL Server Express o LocalDB.
2. Ejecute el script `script.sql` en SQL Server Management Studio o `sqlcmd`:

   ```bash
   sqlcmd -S (localdb)\\MSSQLLocalDB -i script.sql
   ```

   Esto eliminará (si existe) y recreará la base `BiosMessenger` con las tablas, restricciones y procedimientos almacenados requeridos.

3. Ajuste la cadena de conexión en `ModeloEF/App.config` y `Sitio/Web.config` si utiliza un servidor distinto a `(localdb)\MSSQLLocalDB`.

La cadena se resuelve por nombre (`BiosMessengerContext`), evitando valores embebidos en el código.

## Lógica de negocio

- **Validaciones:** `ModeloEF/Validador.cs` garantiza formato del usuario (ocho caracteres alfanuméricos en mayúsculas), contraseña (8 caracteres con mayúsculas, minúsculas y dígitos), mayor de edad, email válido, categorías de tres letras, caducidad mínima al día siguiente y destinatarios sin duplicar ni incluir al remitente.
- **Mensajes:** Las altas usan `spMensaje_Alta` y `spMensaje_AddDestinatario`; el resto de operaciones usan LINQ to Entities. La bandeja de entrada filtra mensajes vencidos para cumplir la regla de visibilidad.
- **Usuarios:** Altas y modificaciones usan LINQ; la baja invoca `spUsuario_Baja` y verifica previamente que no existan mensajes asociados (se inhabilita el borrado en cascada desde `BiosMessengerContext`).
- **Default:** Usa una subconsulta LINQ (`MensajesServicio.ContarMensajesConMasDeCincoDestinatarios`) para contar mensajes con más de cinco destinatarios sin emplear agregados directos.

## Uso del sitio

1. Abra la solución en Visual Studio y ejecute la restauración de paquetes NuGet.
2. Establezca `Sitio` como proyecto de inicio (Web Site).
3. Páginas disponibles:
   - **Default:** muestra totales generales.
   - **ABMUsuarios:** permite alta, modificación y baja respetando todas las validaciones.
   - **AltaMensaje:** registra mensajes utilizando los procedimientos almacenados y validaciones.
   - **Bandeja:** muestra bandejas de entrada y salida; los mensajes caducados sólo son visibles para el remitente.

## Pruebas manuales sugeridas

1. **Usuarios:**
   - Intentar alta con usuario o contraseña inválida para verificar que las validaciones se disparen.
   - Crear un usuario válido y luego modificarlo.
   - Intentar eliminar un usuario con mensajes asociados para comprobar la restricción.
2. **Mensajes:**
   - Crear un mensaje con múltiples destinatarios y verificar que la bandeja de salida lo muestre, mientras que la bandeja de entrada de cada destinatario lo liste hasta su caducidad.
   - Generar mensajes con más de cinco destinatarios y verificar el contador en `Default`.
3. **Caducidad:**
   - Configurar la fecha de caducidad al día siguiente y comprobar que el mensaje deja de verse en la bandeja de entrada una vez vencido.

## Consideraciones

- El contexto deshabilita las eliminaciones en cascada (`BiosMessengerContext`) para cumplir la política "si existen registros asociados no se puede eliminar".
- Los servicios encapsulan la lógica para facilitar su reutilización desde el sitio WebForms.
- Se incluyen comentarios XML y estructuras en español para respetar las convenciones del curso.
- Antes de compilar, ejecute `nuget restore` en la solución para obtener Entity Framework 6.5.1.
