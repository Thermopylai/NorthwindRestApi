# 🧩 Northwind REST API (ASP.NET Core)

Moderni REST API -rajapinta rakennettuna **ASP.NET Core (.NET 10)** -teknologialla Northwind-tietokannan päälle.
Projekti toimii backendinä tulevalle **React frontend -sovellukselle**.

---

## 🚀 Projektin tavoite

Tämän projektin tarkoituksena on:

* toteuttaa täysi REST API Northwind-tietokannalle
* harjoitella modernia backend-arkkitehtuuria
* rakentaa skaalautuva pohja frontend-sovellukselle (React)
* oppia autentikointi, auktorisointi ja optimointi käytännössä

---

## 🛠️ Teknologiat

* **ASP.NET Core (.NET 10)**
* **Entity Framework Core**
* **SQL Server (NorthwindOriginal)**
* **JWT Authentication**
* **ASP.NET Identity**
* **Swagger / OpenAPI**
* **CORS (React frontendia varten)**

---

## 🔐 Autentikointi ja auktorisointi

### Autentikointi

* JWT (Bearer token)
* Login / Register endpointit

### Roolit

* `Admin`
* `User`

### Policy-pohjainen auktorisointi

* Permission-claimit (`customers.read`, `orders.manage`, jne.)
* Policyt määritelty keskitetysti

### Esimerkki

```csharp
[Authorize(Policy = AuthorizationPolicies.CanManageCustomers)]
```

---

## 🧠 Arkkitehtuuri

Projekti noudattaa modulaarista rakennetta:

```
Controllers
Services
DTOs
Projections
Common (Permissions & Policies)
Extensions
Data
```

### 🔹 DTO-mallit

| Tyyppi      | Tarkoitus              |
| ----------- | ---------------------- |
| `CreateDto` | Luonti                 |
| `UpdateDto` | Päivitys               |
| `ReadDto`   | Yksityiskohtainen haku |
| `ListDto`   | Kevyt listaus          |

---

## ⚡ Suorituskykyoptimointi

### Listaus vs Detail

Projektissa käytetään kahta eri query-polkuja:

#### 🔹 Listaus (kevyt)

* `ListDto`
* Ei nested dataa
* Käytössä:

  * `GetAllAsync`
  * `GetPagedAsync`
  * `SearchAsync`

#### 🔹 Detail (raskas)

* `ReadDto`
* Sisältää nested dataa (esim. Orders)
* Käytössä:

  * `GetByIdAsync`

---

## 🔎 Haku, suodatus ja sivutus

Kaikissa tauluissa:

* 🔍 monikenttähaku (`ApplySearch`)
* 🎯 suodatus (`ApplyFilter`)
* 🔃 lajittelu (`ApplySorting`)
* 📄 sivutus (`ToPagedResultAsync`)

---

## 📦 Tuetut resurssit

* Categories
* Customers
* Employees
* Order_Details
* Orders
* Products
* Regions
* Shippers
* Suppliers
* Territories

---

## 🧾 Esimerkki endpointista

### GET /api/customers

Palauttaa kevyen listauksen:

```json
[
  {
    "customerID": "ALFKI",
    "companyName": "Alfreds Futterkiste",
    "city": "Berlin",
    "country": "Germany"
  }
]
```

---

## 🔐 Esimerkki JWT-tokenista

Swaggerissa:

1. Login
2. Kopioi token
3. Klikkaa **Authorize**
4. Liitä token (ilman "Bearer ")

---

## 🌐 CORS

Projektissa on käytössä **kaikki salliva CORS-politiikka** frontend-kehitystä varten:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});
```

---

## 🔮 Tuleva kehitys

Tämän projektin päälle rakennetaan:

👉 **React frontend (seuraavalla kurssilla)**

Suunniteltu arkkitehtuuri tukee:

* erillistä frontend/backend rakennetta
* REST API -kommunikaatiota
* autentikointia JWT:llä

---

## 📌 Kehittäjän huomioita

Projektissa on kiinnitetty erityistä huomiota:

* modulaarisuuteen
* uudelleenkäytettävyyteen
* suorituskykyyn
* laajennettavuuteen

---

## 🧪 Swagger

Swagger UI löytyy osoitteesta:

```
https://localhost:xxxx/
```

---

## 📷 (Lisättävä myöhemmin)

* Swagger screenshot
* API testaus
* Tuleva React UI

---

## 👨‍💻 Tekijä

**Lauri**
Ohjelmistokehityksen opiskelija (Careeria)

---

## 📄 Lisenssi

Tämä projekti on tarkoitettu opiskelukäyttöön.
