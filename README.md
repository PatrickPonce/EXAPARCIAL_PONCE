# EXAPARCIAL_PONCE

Aplicación web desarrollada en **ASP.NET Core MVC** para la gestión de cursos y matrículas en un entorno universitario. Incluye funcionalidades de roles, sesiones con Redis y panel de administración para coordinadores.

---

## URL en producción

La aplicación está desplegada en Render y disponible en:

[https://exaparcial-ponce-1.onrender.com](https://exaparcial-ponce-1.onrender.com)

---

## Funcionalidades

- **Roles y autenticación**
  - Registro y login de usuarios.
  - Rol **Coordinador** con acceso a panel de administración.
  - Acceso denegado para usuarios sin rol.

- **Gestión de cursos**
  - Crear, editar y desactivar cursos (solo Coordinador).
  - Listado de cursos activos.
  - Cache en Redis de cursos por 60 segundos.

- **Gestión de matrículas**
  - Inscripción a cursos.
  - Visualización de matrículas del usuario.
  - Panel de coordinador para confirmar/cancelar matrículas.

- **Sesión con Redis**
  - Guarda el último curso visitado para mostrar enlace dinámico: “Volver al curso {Nombre}”.

---

## Tecnologías

- ASP.NET Core MVC 8.0
- Entity Framework Core (SQLite)
- Identity para gestión de usuarios y roles
- Redis (sesión y cache)
- Bootstrap 5
- Render (despliegue en producción)

---

## Base de datos

- SQLite local para desarrollo.
- Redis para sesiones y cache de cursos.
- La base de datos se inicializa automáticamente al ejecutar la app con datos de prueba y un usuario coordinador por defecto:

  **Coordinador**  
  - Email: `coordinador@usmp.edu.pe`  
  - Contraseña: `Coordinador123!`

---

## Despliegue en Render

1. Clonar el repositorio:  
   ```bash
   git clone https://github.com/PatrickPonce/EXAPARCIAL_PONCE.git
   git checkout deploy/render
