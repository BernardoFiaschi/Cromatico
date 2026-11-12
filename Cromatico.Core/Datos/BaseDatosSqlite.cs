using System.IO;
using Microsoft.Data.Sqlite;

namespace Cromatico.Datos
{
    public sealed class BaseDatosSqlite
    {
        private const string Esquema = @"
            PRAGMA journal_mode = WAL;

            CREATE TABLE IF NOT EXISTS Usuarios (
                Email          TEXT PRIMARY KEY COLLATE NOCASE,
                PasswordHash   TEXT NOT NULL,
                Salt           TEXT NOT NULL,
                FechaRegistro  TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS Configuraciones (
                Clave                  TEXT PRIMARY KEY,
                Version                INTEGER NOT NULL,
                PerfilActivoId         TEXT NOT NULL,
                AutoPerfilesActivo     INTEGER NOT NULL,
                UltimaModificacionUtc  TEXT NOT NULL,
                EvaluacionCompleta     INTEGER NOT NULL,
                TipoDetectada          TEXT NOT NULL,
                NivelDetectado         TEXT NOT NULL,
                Extras                 TEXT NULL
            );

            CREATE TABLE IF NOT EXISTS Perfiles (
                Clave          TEXT NOT NULL REFERENCES Configuraciones(Clave) ON DELETE CASCADE,
                Id             TEXT NOT NULL,
                Orden          INTEGER NOT NULL,
                Nombre         TEXT NOT NULL,
                EsPredefinido  INTEGER NOT NULL,
                Tipo           TEXT NOT NULL,
                Severidad      REAL NOT NULL CHECK (Severidad BETWEEN 0 AND 1),
                Intensidad     REAL NOT NULL CHECK (Intensidad BETWEEN 0 AND 1),
                PRIMARY KEY (Clave, Id)
            );

            CREATE TABLE IF NOT EXISTS Asociaciones (
                Clave     TEXT NOT NULL REFERENCES Configuraciones(Clave) ON DELETE CASCADE,
                Orden     INTEGER NOT NULL,
                Proceso   TEXT NOT NULL,
                Ruta      TEXT NOT NULL,
                PerfilId  TEXT NOT NULL,
                PRIMARY KEY (Clave, Orden)
            );";

        private readonly string _cadenaConexion;

        public string Ruta { get; }

        public BaseDatosSqlite(string? ruta = null)
        {
            Ruta = ruta ?? Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "Cromatico", "cromatico.db");
            Directory.CreateDirectory(Path.GetDirectoryName(Ruta)!);

            _cadenaConexion = new SqliteConnectionStringBuilder
            {
                DataSource = Ruta,
                ForeignKeys = true
            }.ToString();

            using var conexion = Abrir();
            using var comando = conexion.CreateCommand();
            comando.CommandText = Esquema;
            comando.ExecuteNonQuery();
        }

        public SqliteConnection Abrir()
        {
            var conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();
            return conexion;
        }
    }
}
