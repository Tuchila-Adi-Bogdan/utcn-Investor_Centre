<!-- https://github.com/othneildrew/Best-README-Template -->
<a id="readme-top"></a>

[![LinkedIn][linkedin-shield]][linkedin-url]

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="https://github.com/Tuchila-Adi-Bogdan/utcn-Investor_Centre">
    <img src="media/Welcome to the investor Center.png" alt="Logo" width="80" height="80">
  </a>

<h3 align="center">Investor Center</h3>

  <p align="center">
    Un site de tip market unde utilizatorii pot cumpăra/vinde acțiuni, cu preț care fluctuează în timp real. Implementat cu ASP.NET.
    <br />
    Pentru materia IS (inginerie softare). 
    <br />
    <a href="https://github.com/Tuchila-Adi-Bogdan/utcn-Investor_Centre/tree/main/docs"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <label> Nu e deployed.</label>
    <!-- <a href="https://github.com/Tuchila-Adi-Bogdan/utcn-Investor_Centre">View Demo</a> -->
    &middot;
    <a href="https://github.com/Tuchila-Adi-Bogdan/utcn-Investor_Centre/issues/new?labels=bug&template=bug-report---.md">Report Bug</a>
    &middot;
    <a href="https://github.com/Tuchila-Adi-Bogdan/utcn-Investor_Centre/issues/new?labels=enhancement&template=feature-request---.md">Request Feature</a>
  </p>
</div>



<!-- TABLE OF CONTENTS -->
Cuprins
  
1. Scurtă descriere a proiectului
2. Limbaje și tehnologii folosite
3. Cerințe funcționale
4. Cerințe non-funcționale
5. Design Pattern
6. Roadmap
7. Contact
   
<!-- ABOUT THE PROJECT -->
## Descrierea proiectului

Un site de tip market unde utilizatorii pot cumpăra/vinde acțiuni, cu preț care fluctuează în timp real. Implementat cu ASP.NET. Pentru materia IS (inginerie softare). 

## Limbaje și tehnologii folosite

Backend (Server-Side)
- Limbaj: C# (.NET 9.0)
- Framework: ASP.NET Core 8.0 (MVC Pattern)
- Access baza de date: Entity Framework Core (EF Core)
- Autentificare: ASP.NET Core Identity
- Update in timp real: SignalR (Folosit pentru a trimite prețurile acțiunilor în timp real către browser fără refresh)

Baza de date
- Engine baza de date: SQLite
- Management: EF Core Migrations

Frontend (Client-Side)
- View Engine: Razor Views (Fisiere .cshtml care combină C# și HTML)
- Styling: Bootstrap 5 (CSS framework)
- Scripting: JavaScript (Vanilla JS)
- Librărie charting: Chart.js
- Icons/UI: Bootstrap Icons

Development Tools
- IDE: Visual Studio 2022 și Visual Studio 2026
- Package Manager: NuGet

<!-- Cerințe funcționale și non-funcționale -->
## Cerințe funcționale

- Gestionarea Utilizatorilor și Autentificare (Identity)
  - Înregistrare: Utilizatorii noi își pot crea un cont folosind o adresă de email și o parolă.
  - Autentificare (Login): Utilizatorii existenți se pot loga pentru a accesa funcționalitățile de tranzacționare.
  - Deconectare (Logout): Posibilitatea de a părăsi sesiunea curentă în siguranță.
  - Gestionarea Rolurilor: Sistemul distinge între utilizatori obișnuiți ("User") și administratori ("Admin").

- Piața Bursieră și Vizualizare
  - Lista de Acțiuni (Trading Floor): Afișarea tuturor acțiunilor disponibile, împreună cu prețul curent și o mini-diagramă a tendinței recente (Sparkline).
  - Detalii Acțiune: O pagină dedicată pentru fiecare acțiune care afișează:
    - Prețul curent actualizat în timp real.
    - Grafic interactiv (Chart.js) cu istoricul prețurilor.
    - Selector de intervale de timp (1 minut, 1 oră, 1 zi, etc.).
    - Indicateurs de Trend: Vizualizarea modificărilor procentuale (verde pentru creștere, roșu pentru scădere).

- Tranzacționare (Trading)
  - Cumpărare (Buy):
    - Utilizatorii pot cumpăra acțiuni dacă au suficient sold.
    - Sistemul calculează costul total și îl scade din balanța utilizatorului.
    - Dacă utilizatorul deține deja acțiunea, cantitatea se actualizează și se recalculează prețul mediu de achiziție.
  - Vânzare (Sell):
    - Utilizatorii pot vinde acțiuni doar dacă dețin cantitatea necesară în portofoliu.
    - Sistemul adaugă valoarea vânzării în balanța utilizatorului.
    - Validări: Prevenirea cumpărării fără fonduri insuficiente și a vânzării acțiunilor inexistente.

- Portofoliu (Portfolio)
  - Vizualizare Dețineri: Utilizatorul poate vedea o listă cu toate acțiunile pe care le deține.
  - Calcul Valoare Totală: Afișarea valorii curente a fiecărei poziții (Preț Curent × Cantitate) și a valorii totale a portofoliului.
  - Navigare Rapidă: Link-uri directe către pagina de detalii a acțiunii pentru a iniția noi tranzacții.

- Funcționalități în Timp Real (SignalR)
  - Actualizare Prețuri: Prețurile acțiunilor se modifică automat pe ecranul utilizatorului fără a fi nevoie de refresh (reîncărcarea paginii).
  - Actualizare Grafice: Graficele primesc puncte noi de date pe măsură ce prețurile se schimbă.

- Administrare (Admin Panel)
  - Adăugare Acțiuni: Administratorul poate introduce noi companii în sistem (Ticker, Nume, Preț inițial).
  - Gestionare Știri: Administratorul poate publica și șterge articole de știri care pot influența prețul acțiunilor (bazat pe structura bazei de date cu NewsArticle).
  - Editare/Ștergere: Posibilitatea de a modifica detalii despre acțiuni sau de a șterge conținut.

## Cerințe Non-Funcționale

Acestea definesc atributele de calitate, performanță și constrângerile tehnice.
- Performanță și Eficiență
  - Latență Scăzută: Utilizarea SignalR (WebSockets) asigură transmiterea prețurilor instantaneu de la server la toți clienții conectați.
  - Randare Asincronă: Utilizarea async/await în C# pentru a nu bloca serverul în timpul interogărilor bazei de date.
  - Încărcare Grafică Client-Side: Graficele sunt generate în browserul utilizatorului folosind Chart.js, reducând încărcarea pe server.

- Securitate
  - Autentificare Securizată: Utilizarea ASP.NET Core Identity pentru hashing-ul parolelor și managementul sesiunilor (Cookies).
  - Autorizare: Protejarea rutelor sensibile (ex: /Portfolio, /Admin) prin atributele [Authorize] și [Authorize(Roles = "Admin")]. Utilizatorii neautentificați nu pot accesa sau manipula date.
  - Protecție CSRF: Formularele includ automat token-uri anti-falsificare (Anti-Forgery Tokens) generate de ASP.NET Core.

- Utilizabilitate (Usability)
  - Interfață Responsive: Utilizarea framework-ului Bootstrap 5 asigură că site-ul arată bine pe desktop, tablete și telefoane mobile.
  - Feedback Vizual: Sistemul oferă mesaje clare de succes sau eroare (ex: "Fonduri insuficiente", "Acțiune cumpărată cu succes") prin TempData.
  - Localizare: Sistemul este forțat pe cultura en-US pentru a afișa corect moneda ($) și formatele numerice, indiferent de locația utilizatorului.

- Fiabilitate și Integritate a Datelor
  - Consistența Tranzacțiilor: Operațiunile de cumpărare/vânzare sunt atomice (se întâmplă complet sau deloc) datorită tranzacțiilor bazei de date.
  - Integritate Referențială: Baza de date (SQLite) folosește chei străine (Foreign Keys) pentru a lega Portofoliul de Utilizator și de Acțiuni, prevenind datele orfane.

<!-- Design Pattern -->
### Design Patterns
#### Exemplu: Dependency Injection (DI)
Problema: Controller-ul are nevoie de acces la baza de date (context) și la sistemul de useri (userManager) pentru a funcționa. Abordarea greșită (fără pattern): Am fi scris: 
```
var context = new InvestorCenterContext();
```
în interiorul fiecărei metode. Asta ar fi creat probleme de performanță și ar fi făcut codul greu de testat.
Soluția (Design Pattern): Nu creez obiectele cu new, ci le cer în constructor. Framework-ul ASP.NET (care se comportă ca un "Container") vede că am nevoie de ele, le creează automat în fundal și le "injectează", gata de folosire.
```
public class PortfolioController : Controller
{
    // Acestea sunt "dependențele"
    private readonly InvestorCenterContext _context;
    private readonly UserManager<InvestorCenterUser> _userManager;

    // CONSTRUCTORUL: Aici are loc "Injecția"
    public PortfolioController(InvestorCenterContext context, UserManager<InvestorCenterUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }
}
```
<!-- ROADMAP -->
## Roadmap

- [x] Login Framework
- [x] Database
- [x] Financial News
- [x] Stocks Tab
    - [x] Invidiual Stock Details
- [x] Wallet
- [x] Buy/Sell Stocks
- [x] News Effects
- [x] Owned Stocks Page
- [x] Better checks when admin adds stocks/news

<!-- CONTACT -->
## Contact

Email - adibogdan2004@gmail.com

Project Link: [https://github.com/Tuchila-Adi-Bogdan/utcn-Investor_Centre](https://github.com/Tuchila-Adi-Bogdan/utcn-Investor_Centre)

<p align="right">(<a href="#readme-top">back to top</a>)</p>



<!-- MARKDOWN LINKS & IMAGES -->
[linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=for-the-badge&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/https://www.linkedin.com/in/adi-tuchila-02770225a/
[product-screenshot]: images/screenshot.png
