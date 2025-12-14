# FacturacionBG

Sistema de **Facturación** desarrollado con **.NET 8 (Backend API)** y **Angular (Frontend)**, orientado a la gestión de facturas, productos, clientes y usuarios, con control de acceso basado en **roles**.

> 📌 **Importante**: El **backend y el frontend se encuentran en el mismo repositorio**.

---

## 🚀 Funcionalidades Principales

* Gestión de **Facturas**
* Gestión de **Productos**
* Gestión de **Clientes**
* Gestión de **Usuarios y Roles**
* Autenticación y autorización basada en **JWT**
* Control de acceso por **roles**

---

## 🔐 Roles y Permisos

El sistema maneja **3 roles principales**:

### 👑 Administrator

Rol con **acceso total al sistema**.

Permisos:

* Acceso completo a **Configuraciones**
* Gestión de **Usuarios**

  * Crear usuarios con **cualquier rol** (Administrator, Seller, Customer)
* Acceso completo a:

  * Facturas
  * Productos
  * Clientes

---

### 🧾 Seller

Rol operativo del sistema.

Permisos:

* Acceso completo a:

  * Facturas
  * Productos
  * Clientes
* ❌ No tiene acceso a **Configuraciones**
* ❌ No puede crear usuarios con roles administrativos

---

### 👤 Customer

Rol de cliente final.

Permisos:

* ✅ **Solo puede visualizar las facturas asociadas a su propio usuario**
* ❌ No puede crear, editar o eliminar información
* ❌ No tiene acceso a Productos, Clientes ni Configuraciones

---

## 🧑‍💻 Creación de Usuarios

Existen **dos formas de crear usuarios** en el sistema:

### 1️⃣ Creación por Administrator

* Solo un usuario con rol **Administrator** puede crear usuarios
* Puede asignar **cualquier rol**:

  * Administrator
  * Seller
  * Customer

### 2️⃣ Registro público (sin estar logueado)

* Disponible desde el frontend
* El usuario creado **automáticamente recibe el rol Customer**

---

## 🧱 Arquitectura del Proyecto

```
FacturacionBG/
│
├── Backend/       # API .NET 8
│   ├──src/
│   │   ├──API/
│   │   ├──Application/
│   │   ├──Domain/
│   │   └──Infrastructure/
│   └──scripts/
├── Frontend/       # Angular
├── docker-compose.prod.yml
└── README.md
```

---

## 📋 Requerimientos

### Requerimientos Generales

* Node.js (v18 o superior recomendado)
* Angular CLI
* .NET SDK 8
* SQL Server
* Docker Desktop (opcional, para despliegue)

---

## ⚙️ Instalación del Proyecto (Sin Docker)

### 🔹 Backend (.NET 8 API)

#### 1️⃣ Configurar `appsettings.json`

Editar el archivo `appsettings.json` del proyecto API y configurar la cadena de conexión:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVER;Database=FacturacionBG;User ID=TU_USER;Password=TU_PASSWORD;TrustServerCertificate=True"
}
```

Valores a configurar:

* `Server`
* `User ID`
* `Password`

#### 2️⃣ Ejecutar el Backend

* Abrir la solución principal del backend
* Ejecutar el proyecto API

Swagger disponible en:

```
https://localhost:7223/swagger/index.html
```

---

### 🔹 Frontend (Angular)

#### 1️⃣ Instalación de dependencias

Ubicados en la carpeta del frontend:

```bash
npm install
```

#### 2️⃣ Ejecutar el proyecto

```bash
ng serve
```

La aplicación estará disponible en:

```
http://localhost:4200
```

---

### 🌐 Configuración de CORS

Si existen problemas de CORS:

1. Ir al `Program.cs` del backend
2. Verificar la configuración de CORS
3. Asegurarse de permitir el origen del frontend:

```csharp
policy.WithOrigins("http://localhost:4200")
      .AllowAnyHeader()
      .AllowAnyMethod();
```

4. Verificar que la URL del backend sea correcta en:

```
Frontend/src/environments/environment.ts
```

```ts
export const environment = {
  apiUrl: 'https://localhost:7223/api'
};
```

---

## 🐳 Instalación con Docker (Producción)

Este proyecto utiliza **Docker Compose para producción**, levantando contenedores a partir de **imágenes ya publicadas en la nube**.

### 📌 Prerrequisitos

* Docker Desktop instalado
* Acceso a las imágenes Docker publicadas

---

### ▶️ Levantar el Proyecto

Ubicados en la raíz del proyecto, ejecutar:

```bash
docker-compose -f docker-compose.prod.yml up -d
```

Este comando:

* Descarga las imágenes desde el registry
* Levanta los contenedores definidos en `docker-compose.prod.yml`
* Ejecuta el sistema en segundo plano

---

## ✅ Resultado Final

Una vez levantado:

* Frontend disponible según el puerto configurado (ej: `http://localhost:4200`)
* Backend accesible vía API
* Autenticación y autorización funcionando según los **roles definidos**

---

## 📄 Notas Finales

* El acceso a cada módulo está **estrictamente controlado por roles**
* El sistema está preparado para escalar y agregar nuevos permisos
* Ideal para entornos de **facturación, ventas y control administrativo**

---

✍️ *Proyecto FacturacionBG*
