using System.Globalization;
using System.Text.Json;
using Cromatico.Modelos;
using Microsoft.Data.Sqlite;

namespace Cromatico.Datos
{
    public class RepositorioConfiguracionSqlite : IRepositorioConfiguracion
    {
        private readonly BaseDatosSqlite _db;

        public RepositorioConfiguracionSqlite(BaseDatosSqlite db)
        {
            _db = db;
        }

        public string Ubicacion => _db.Ruta;

        public ConfiguracionApp? Leer(string clave)
        {
            using var conexion = _db.Abrir();

            ConfiguracionApp config;
            using (var comando = Comando(conexion, null, @"
                SELECT Version, PerfilActivoId, AutoPerfilesActivo, UltimaModificacionUtc,
                       EvaluacionCompleta, TipoDetectada, NivelDetectado, Extras
                FROM Configuraciones
                WHERE Clave = $clave"))
            {
                comando.Parameters.AddWithValue("$clave", clave);
                using var lector = comando.ExecuteReader();
                if (!lector.Read()) return null;

                config = new ConfiguracionApp
                {
                    Version = lector.GetInt32(0),
                    PerfilActivoId = lector.GetString(1),
                    AutoPerfilesActivo = lector.GetInt64(2) != 0,
                    UltimaModificacionUtc = LeerFecha(lector.GetString(3)),
                    EvaluacionCompleta = lector.GetInt64(4) != 0,
                    TipoDetectada = Enum.Parse<TipoDeficiencia>(lector.GetString(5)),
                    NivelDetectado = Enum.Parse<NivelSeveridad>(lector.GetString(6)),
                    Extras = lector.IsDBNull(7)
                        ? null
                        : JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(lector.GetString(7))
                };
            }

            using (var comando = Comando(conexion, null, @"
                SELECT Id, Nombre, EsPredefinido, Tipo, Severidad, Intensidad
                FROM Perfiles
                WHERE Clave = $clave
                ORDER BY Orden"))
            {
                comando.Parameters.AddWithValue("$clave", clave);
                using var lector = comando.ExecuteReader();
                while (lector.Read())
                {
                    config.Perfiles.Add(new PerfilCorreccion
                    {
                        Id = lector.GetString(0),
                        Nombre = lector.GetString(1),
                        EsPredefinido = lector.GetInt64(2) != 0,
                        Tipo = Enum.Parse<TipoDeficiencia>(lector.GetString(3)),
                        Severidad = lector.GetDouble(4),
                        Intensidad = lector.GetDouble(5)
                    });
                }
            }

            using (var comando = Comando(conexion, null, @"
                SELECT Proceso, Ruta, PerfilId
                FROM Asociaciones
                WHERE Clave = $clave
                ORDER BY Orden"))
            {
                comando.Parameters.AddWithValue("$clave", clave);
                using var lector = comando.ExecuteReader();
                while (lector.Read())
                {
                    config.Asociaciones.Add(new AsociacionAplicacion
                    {
                        Proceso = lector.GetString(0),
                        Ruta = lector.GetString(1),
                        PerfilId = lector.GetString(2)
                    });
                }
            }

            return config;
        }

        // Todo en una transacción: o se guarda la configuración completa o no cambia nada.
        public void Escribir(string clave, ConfiguracionApp configuracion)
        {
            using var conexion = _db.Abrir();
            using var transaccion = conexion.BeginTransaction();

            using (var comando = Comando(conexion, transaccion, @"
                INSERT INTO Configuraciones (Clave, Version, PerfilActivoId, AutoPerfilesActivo, UltimaModificacionUtc,
                                             EvaluacionCompleta, TipoDetectada, NivelDetectado, Extras)
                VALUES ($clave, $version, $activo, $auto, $fecha, $evaluacion, $tipoDetectado, $nivelDetectado, $extras)
                ON CONFLICT(Clave) DO UPDATE SET
                    Version = excluded.Version,
                    PerfilActivoId = excluded.PerfilActivoId,
                    AutoPerfilesActivo = excluded.AutoPerfilesActivo,
                    UltimaModificacionUtc = excluded.UltimaModificacionUtc,
                    EvaluacionCompleta = excluded.EvaluacionCompleta,
                    TipoDetectada = excluded.TipoDetectada,
                    NivelDetectado = excluded.NivelDetectado,
                    Extras = excluded.Extras"))
            {
                comando.Parameters.AddWithValue("$clave", clave);
                comando.Parameters.AddWithValue("$version", configuracion.Version);
                comando.Parameters.AddWithValue("$activo", configuracion.PerfilActivoId);
                comando.Parameters.AddWithValue("$auto", configuracion.AutoPerfilesActivo ? 1 : 0);
                comando.Parameters.AddWithValue("$fecha", configuracion.UltimaModificacionUtc.ToString("O", CultureInfo.InvariantCulture));
                comando.Parameters.AddWithValue("$evaluacion", configuracion.EvaluacionCompleta ? 1 : 0);
                comando.Parameters.AddWithValue("$tipoDetectado", configuracion.TipoDetectada.ToString());
                comando.Parameters.AddWithValue("$nivelDetectado", configuracion.NivelDetectado.ToString());
                comando.Parameters.AddWithValue("$extras",
                    configuracion.Extras == null ? DBNull.Value : JsonSerializer.Serialize(configuracion.Extras));
                comando.ExecuteNonQuery();
            }

            Borrar(conexion, transaccion, "DELETE FROM Perfiles WHERE Clave = $clave", clave);
            Borrar(conexion, transaccion, "DELETE FROM Asociaciones WHERE Clave = $clave", clave);

            for (int i = 0; i < configuracion.Perfiles.Count; i++)
            {
                var p = configuracion.Perfiles[i];
                using var comando = Comando(conexion, transaccion, @"
                    INSERT INTO Perfiles (Clave, Id, Orden, Nombre, EsPredefinido, Tipo, Severidad, Intensidad)
                    VALUES ($clave, $id, $orden, $nombre, $predef, $tipo, $sev, $int)");
                comando.Parameters.AddWithValue("$clave", clave);
                comando.Parameters.AddWithValue("$id", p.Id);
                comando.Parameters.AddWithValue("$orden", i);
                comando.Parameters.AddWithValue("$nombre", p.Nombre);
                comando.Parameters.AddWithValue("$predef", p.EsPredefinido ? 1 : 0);
                comando.Parameters.AddWithValue("$tipo", p.Tipo.ToString());
                comando.Parameters.AddWithValue("$sev", p.Severidad);
                comando.Parameters.AddWithValue("$int", p.Intensidad);
                comando.ExecuteNonQuery();
            }

            for (int i = 0; i < configuracion.Asociaciones.Count; i++)
            {
                var a = configuracion.Asociaciones[i];
                using var comando = Comando(conexion, transaccion, @"
                    INSERT INTO Asociaciones (Clave, Orden, Proceso, Ruta, PerfilId)
                    VALUES ($clave, $orden, $proceso, $ruta, $perfil)");
                comando.Parameters.AddWithValue("$clave", clave);
                comando.Parameters.AddWithValue("$orden", i);
                comando.Parameters.AddWithValue("$proceso", a.Proceso);
                comando.Parameters.AddWithValue("$ruta", a.Ruta ?? string.Empty);
                comando.Parameters.AddWithValue("$perfil", a.PerfilId);
                comando.ExecuteNonQuery();
            }

            transaccion.Commit();
        }

        private static SqliteCommand Comando(SqliteConnection conexion, SqliteTransaction? transaccion, string sql)
        {
            var comando = conexion.CreateCommand();
            comando.Transaction = transaccion;
            comando.CommandText = sql;
            return comando;
        }

        private static void Borrar(SqliteConnection conexion, SqliteTransaction transaccion, string sql, string clave)
        {
            using var comando = Comando(conexion, transaccion, sql);
            comando.Parameters.AddWithValue("$clave", clave);
            comando.ExecuteNonQuery();
        }

        private static DateTime LeerFecha(string texto)
            => DateTime.Parse(texto, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
    }
}
