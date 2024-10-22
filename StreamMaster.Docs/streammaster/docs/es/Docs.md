# 📘 Documentación de StreamMaster

Este documento describe los pasos para configurar y administrar la documentación para el proyecto [StreamMaster] (https://github.com/senexcrenshaw/streammaster) utilizando MKDOCS.La configuración incluye soporte para la internacionalización (I18N) y el uso del tema del material MKDOCS para una apariencia moderna.

## ¿Por qué contribuir a la documentación?

La documentación es lo primero a lo que los usuarios y desarrolladores se refieren cuando usan o contribuyen a StreamMaster.Al ayudar a mejorar y mantener la documentación, está asegurando que StreamMaster permanezca accesible y fácil de usar para la comunidad.

Sus contribuciones a la documentación:
- Ayude a otros a aprender y usar StreamMaster de manera más efectiva.
- Mejore la claridad para los hablantes de inglés no nativos a través de un mejor soporte de I18n.
- Permitir que los desarrolladores contribuyan más fácilmente al proyecto.

¡Incluso las pequeñas actualizaciones como corregir errores tipográficos o instrucciones de aclaración hacen una gran diferencia!

## 🛠 Requisitos previos

Para generar y servir documentación localmente, necesitará Python instalado.Asegúrese de que 'Pip`, el Administrador de paquetes de Python, también esté disponible.

### Instalación de MKDOC y complementos

Para instalar MKDOC y los complementos requeridos para i18n y temas, ejecute el siguiente comando:

`` `Bash
Python -M PIP Instale MKDOCS MKDOCS-I18N MKDOCS-MATERIAL MKDOCS-STATIC-I18N
`` `` ``

Esto instala lo siguiente:

- `MKDOCS`: el generador de sitios estáticos.
- `MKDOCS-I18N`: para manejar la internacionalización.
- `Mkdocs-Material`: un tema popular con un diseño moderno.
-`Mkdocs-static-i18n`: agrega soporte de internacionalización estática.

## Desarrollo local

Para construir y servir la documentación localmente durante el desarrollo, siga estos pasos.

### Construyendo los documentos

Para generar la documentación estática:

`` `Bash
construcción de mkdocs
`` `` ``

### Sirviendo a los documentos localmente

Para ejecutar un servidor de desarrollo que observa los cambios y se vuelve a cargar automáticamente:

`` `Bash
Mkdocs sirve
`` `` ``

Esto alojará la documentación localmente en `http: // localhost: 8000`.

## construcción de producción

Cuando esté listo para implementar la documentación en la producción, asegúrese de limpiar la compilación anterior y establecer la URL del sitio correctamente.Ejecute el siguiente comando:

`` `Bash
MKDOCS Build --Clean --Site-URL https://senexcrenshaw.github.io/streammaster/
`` `` ``

Esto crea una versión limpia de la documentación y establece la URL correcta para el sitio de producción.

## 📝 Contribuyendo a la documentación

Los archivos de documentación en vivo en la carpeta `streammaster.docs \ streammaster` en la [Repository Streammaster] (https://github.com/senexcrenshaw/streammaster).

Para contribuir a la documentación:

- ** Crea una nueva rama ** para tus cambios.
- ** Asegúrese de que el inglés (`en`) siempre se actualice **.El inglés sirve como idioma principal, y todo el contenido debe actualizarse en inglés.
- ** Proporcione las mejores traducciones posibles ** para otros idiomas compatibles, como el español (`Es`), el francés (` fr`), alemán (`de`) y cualquier otro idioma apoyado.Si bien estas traducciones no tienen que ser perfectas, deben apuntar a reflejar con precisión el contenido de inglés.
- Inglés archivos en vivo bajo `docs/en`.
- Las traducciones viven bajo sus respectivos directorios (por ejemplo, `docs/es` para español,` docs/fr` para francés, etc.).
- ** Prueba ** Todos los cambios ejecutando `mkdocs sirviendo 'localmente tanto para la versión en inglés como para cualquier traducción actualizada.
- ** Envíe una solicitud de extracción (PR) ** para su revisión.

### ¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡¡.

1. Bifurca el repositorio y clona a su máquina local.
2. Cree una nueva rama para sus cambios.
3. Asegúrese de que el inglés (`en`) se actualice y proporcione las mejores traducciones posibles para otros idiomas compatibles, luego envíe un PR.

¡Eso es todo!🎉 ¡Has contribuido a StreamMaster!

## consejos para escribir una buena documentación

- ** Sea claro y conciso: ** Concéntrese en los puntos principales y evite el lenguaje demasiado técnico cuando sea posible.
- ** Use ejemplos: ** Los fragmentos de código o las ayudas visuales ayudan a aclarar conceptos complejos.
- ** Sea consistente: ** Mantenga el tono y la terminología consistentes en todas las secciones.
- ** Pruebe todo: ** Asegúrese de que todos los ejemplos, comandos e instrucciones de código funcionen como se esperaba.

## Reconocimiento de contribuyentes 🌟

¡Apreciamos cada contribución, no importa cuán pequeño sea!Todos los contribuyentes se agregarán al Salón de la Fama de la documentación de Streammaster, que se presenta a continuación:

[Ver todos los contribuyentes] (contribuyentes.md)

## ¡Necesitamos tu ayuda!🤝

Streammaster está creciendo constantemente, y necesitamos la ayuda de la comunidad para mantener la documentación de primera categoría.Ninguna contribución es demasiado pequeña, ya sea arreglando errores tipográficos, agregar ejemplos o traducir contenido.

¡Salta y hagamos que Streammaster sea mejor juntos!✨

## ¿Necesita ayuda o tiene preguntas?¡Únete a nosotros en Discord!🎮

Para cualquier pregunta, soporte o discusión, puede unirse al servidor oficial ** Streammaster Discord **.

👉 [Únete a Streammaster Discord] (https://discord.gg/gfz7ethhg2) 👈

Estamos aquí para ayudar, y encontrará una comunidad activa de desarrolladores y usuarios.¡No dude en hacer preguntas, informar cuestiones o discutir nuevas ideas para mejorar StreamMaster!