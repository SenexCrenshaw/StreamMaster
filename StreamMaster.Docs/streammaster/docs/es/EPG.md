# EPG/XML Files

## ¿Qué es un archivo EPG/XML?📘

An **EPG (Electronic Program Guide)** file, often formatted in XML (specifically XMLTV format), is used to provide program guide information for streaming channels. EPG files list details about scheduled programs, such as titles, start and end times, descriptions, and genres. This guide data enhances the viewing experience by allowing users to browse current and upcoming shows in a structured format.

In **StreamMaster (SM)**, EPG/XML files can be imported to integrate program guides with your IPTV channels, enriching the user experience with program schedules and metadata.

## Importing EPG/XML Files in StreamMaster 🛠

Para agregar archivos EPG/XML a StreamMaster, use la opción ** Importar EPG ** dentro de la interfaz de usuario de StreamMaster.StreamMaster proporciona varias opciones durante la importación para personalizar cómo se procesan y se muestran los datos de EPG.A continuación se presentan las opciones principales disponibles al importar un archivo EPG.

### Opciones de importación de EPG

|Opción |Descripción |
|------------------- |------------------------------------------------------------------------------------------- |
|** Nombre ** |El nombre que desea asignar al archivo EPG importado.|
|** Nombre del archivo ** |El nombre local para guardar el archivo en una vez importado.|
|** Número de EPG ** |Un identificador único para el archivo EPG, que permite la diferenciación entre múltiples fuentes de EPG.|
|** Cambio de tiempo ** |(Opcional) Ajusta el tiempo de todos los programas por el número especificado de horas.|
|** Horas para actualizar ** |Intervalo (en horas) para verificar y actualizar automáticamente el archivo EPG.|
|** Fuente de URL/Archivo ** |La URL o la ruta del archivo local del archivo EPG se importarán.|
|** Color ** |(Opcional) asigna un color a las entradas de guía para una identificación más fácil en la interfaz.|

### Import Process Overview

Once an EPG/XML file is added using these options, StreamMaster:

1. ** Valida ** - Asegura que sea accesible la URL o fuente de archivo local proporcionada.
2. ** Obtener contenido de obtención y analizador ** - Descarga y lee el contenido XML para completar la información de la guía.
3. ** procesa datos de EPG ** - Ajusta las zonas horarias, aplica colores y vincula programas a canales según la configuración del usuario.
4. ** Guarda y actualizaciones ** - El archivo EPG se guarda dentro del repositorio de StreamMaster, actualizando los datos del programa periódicamente como se especifica.

### Manejo de errores 🔄

During the import, StreamMaster performs several checks to verify the EPG file:

- If the **URL source** is invalid or inaccessible, an error message is displayed, and the file is not added.
- If the **file format** is unsupported or unreadable, StreamMaster stops further processing and alerts the user.
- **Automatic cleanup** occurs on failure to ensure no incomplete files are left in the system.

### Solución de problemas de problemas de importación

If an import fails, check the following:

- Verifique que la ruta de URL o archivo ** ** es correcta y accesible.
- Confirme que el número ** EPG ** es único y no está en uso por otros archivos EPG.
- Asegúrese de que se llenen ** los campos requeridos ** (como el nombre y la URLSource).

### Automating EPG File Refresh 🚀

StreamMaster can automatically update and refresh EPG files. Set the `HoursToUpdate` option to specify the frequency of refresh, keeping your guide data current without manual intervention.

---

{%
    include-markdown "../includes/_footer.md"
%}
