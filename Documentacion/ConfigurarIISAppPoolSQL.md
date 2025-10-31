# Configurar IIS para usar autenticación integrada con SQL Server

Este proyecto está preparado para conectarse a SQL Server mediante **autenticación integrada** desde IIS. Sigue estos pasos para que el *Application Pool* pueda acceder correctamente a la base de datos `BiosMessenger`.

## 1. Verifica dónde se ejecutan IIS y SQL Server

- **Mismo servidor**: puedes usar la identidad virtual `IIS APPPOOL\\<NombreDelPool>`.
- **Servidores distintos**: crea una cuenta de dominio (o local) real y asigna esa cuenta al *application pool*. En SQL Server deberás crear el login para esa cuenta en lugar de la identidad virtual.

Si intentas registrar `IIS APPPOOL\\DefaultAppPool` desde un servidor diferente al que hospeda IIS obtendrás el error `Msg 15401 ... not found`.

## 2. Crear el login en SQL Server

Conéctate a SQL Server **desde el equipo donde corre SQL** con un usuario administrador y ejecuta:

```sql
USE master;
GO
CREATE LOGIN [IIS APPPOOL\DefaultAppPool] FROM WINDOWS;
GO

USE BiosMessenger;
GO
CREATE USER [IIS APPPOOL\DefaultAppPool] FOR LOGIN [IIS APPPOOL\DefaultAppPool];
EXEC sys.sp_addrolemember 'db_owner', [IIS APPPOOL\DefaultAppPool];
GO
```

- Sustituye `DefaultAppPool` por el nombre del *application pool* real si es distinto.
- Si SQL Server está en otro equipo, reemplaza el nombre por la cuenta que hayas asignado al pool (por ejemplo, `DOMINIO\CuentaServicio`).

## 3. Asignar la identidad correcta al Application Pool

1. Abre el Administrador de IIS.
2. En **Application Pools**, ubica el pool de tu sitio.
3. Selecciona **Advanced Settings...** y revisa la propiedad **Identity**.
   - Si usas la identidad virtual, déjala como `ApplicationPoolIdentity`.
   - Si necesitaste crear una cuenta real, cambia la identidad a **Custom account...** y asigna la credencial.
4. Recicla el pool o reinicia IIS para aplicar el cambio.

## 4. Configurar la cadena de conexión

Las cadenas de conexión en `Sitio/Web.config` y `ModeloEF/App.config` están definidas así:

```xml
<add name="BiosMessengerContext"
     connectionString="Data Source=SERVIDOR_SQL;Initial Catalog=BiosMessenger;Integrated Security=True;MultipleActiveResultSets=True"
     providerName="System.Data.SqlClient" />
```

- Reemplaza `SERVIDOR_SQL` por el nombre o instancia de tu servidor.
- Al usar `Integrated Security=True`, SQL Server autenticará la petición con la identidad que ejecuta el pool de IIS.

## 5. Prueba de conectividad

Desde el servidor web ejecuta, con privilegios elevados:

```powershell
Test-Path "\\\SERVIDOR_SQL\c$"  # Verifica resolución de nombre (opcional)
sqlcmd -S SERVIDOR_SQL -d BiosMessenger -E -Q "SELECT CURRENT_USER;"
```

Deberías obtener como resultado el usuario `IIS APPPOOL\DefaultAppPool` (o la cuenta personalizada). Si ves un error 15401, revisa que el nombre coincide exactamente y que SQL Server puede resolver la identidad.

## 6. Consideraciones de seguridad

- Asigna solamente los roles necesarios. Si la aplicación solo lee datos, reemplaza `db_owner` por `db_datareader` y habilita permisos específicos de escritura según sea necesario.
- Protege el acceso a la consola de SQL Server, ya que cualquier administrador de la máquina puede modificar los permisos del login.
- Registra los cambios en el control de versiones y en los procedimientos operativos para facilitar despliegues futuros.

Con estos pasos la aplicación podrá autenticarse en SQL Server usando la identidad del *application pool* y se evita almacenar credenciales de SQL en la configuración.
