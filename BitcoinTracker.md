# BitcoinTracker WPF Graph Extension – Project Documentation

## 1. Overview
The project is built on the **Model-View-ViewModel (MVVM)** architecture, ensuring a clean separation of concerns. The visual rendering of the data uses **only WPF built-in components** (`Canvas` and `Polyline`), avoiding reliance on external graph libraries.

---

## 2. Goals 🎯
* Display historical Bitcoin price and volume data fetched from the CoinGecko API.
* Render data visually within the WPF application using proprietary (built-in) drawing logic.
* Strictly adhere to the MVVM pattern for testability and maintainability.
* Ensure the codebase is simple, readable, and comprehensively documented with C# single line comments.

---

## 3. Architecture 🏗️

### Models (The Data Structure)
| Class | Purpose | Notes |
| :--- | :--- | :--- |
| `DailySummary` | Represents the clean, processed data point for a single day (Date, Price, Volume). | The primary object used for binding and charting. |
| `DailyData` | Represent a single day's criptocurreny market data. | Supports Analyzer class methods calculations. |
| `Analyzer` | Provides methods to analyze criptocurrency data. | Analyzing longest bearish streak, highest trading volume and best day to buy and sell to maximise the profit. |

### Services (The Data Source)
| Class | Purpose | Separation of Concerns |
| :--- | :--- | :--- |
| `ApiClient` | **Low-level Communication.** Handles HTTP requests, response checking, and authentication. | Returns the raw JSON string. |
| `BitcoinService` | **High-level Business Logic.** Constructs API endpoints, performs JSON deserialization, and transforms data into `DailySummary` objects. | Processes and cleans the data. |

For additional context, refer to **[Code Breakdown: Separating Concerns (API vs. Service)](#code-breakdown-separating-concerns-api-vs-service)** below.

### ViewModels (The Mediator)
| Class | Purpose |
| :--- | :--- |
| `MainViewModel` | Exposes data from the Models/Services to the View. It manages asynchronous data fetching, updates the `ObservableCollection<DailySummary>`, and holds properties/commands bound to the `MainWindow`. |

### Views (The Presentation)
* `MainWindow.xaml`: Defines the UI structure.
* Uses a data-bound `ItemsControl` or directly manipulates the `Canvas.Children` collection to draw `Polyline` and other geometrical shapes representing the graph.

---

## 4. Workflow / Työvaiheet ⚙️
1.  **Project Setup**
    * Create solution and standard MVVM folder structure.
2.  **API and Service Layer**
    * Implement `ApiClient` for robust `HttpClient` calls and error handling.
    * Implement `BitcoinService` to manage CoinGecko API date ranges and data transformation.
    * Define the clean `DailySummary` model.
3.  **ViewModel Implementation**
    * Implement `MainViewModel` with a property of type `ObservableCollection<DailySummary>`.
    * Implement an asynchronous command to fetch and load data into the collection.
4.  **Graph Rendering (View)**
    * Design the `MainWindow.xaml` to include a `Canvas`.
    * Implement WPF logic to bind the `DailySummary` collection and use the `PriceDataProcessor` to convert values (price, date) into `PointCollection` coordinates for `Polyline`.
5.  **Refinement and Testing**
    * Add labels, gridlines, and dynamic scaling logic.
    * Conduct unit and integration tests.
6.  **Finalization**
    * Verify data updates and rendering performance.
    * **Push to Git.**

---

## 5. Technologies 🚀
* **C# / WPF** (Desktop Application Framework)
* **.NET 8** (Runtime)
* **MVVM** (Architectural Pattern)
* **System.Text.Json** (Preferred JSON Deserialization/Serialization)
* **CoinGecko API** (External Data Source)

## 6. Comments 💬
All C# files include inline comments explaining:
* Class and method purpose.
* Key variable roles.
* Complex logic (e.g., date-to-pixel conversion, price normalization).

## Output

![image info](./Kuvat/result(1).png)

---

## 📁 Code Breakdown: Separating Concerns (API vs. Service)

The application uses a standard design pattern where responsibilities are separated: `ApiClient` handles low-level networking, and `BitcoinService` handles high-level business logic and data processing.

---

### 🚀 1. `ApiClient.cs` (The Communicator)

This class acts as the **dedicated network interface**. Its sole job is to manage the HTTP connection and fetch the raw data from the external API.

| Aspect | Description | Key Code Lines |
| :--- | :--- | :--- |
| **Primary Role** | **Handles all network requests (HTTP GET)** and connection setup with the CoinGecko API. | `_httpClient = new HttpClient { ... };` |
| **Setup** | Initializes the reusable `HttpClient`, sets the `BaseAddress` (`https://api.coingecko.com/api/v3/`), and adds the API key to the request headers. | `_httpClient.DefaultRequestHeaders.Add(...)` |
| **Core Method** | `GetAsync(string endpoint)` | Executes the request using `await _httpClient.GetAsync(endpoint)`. |
| **Error Handling** | Checks for a successful response code (`!response.IsSuccessStatusCode`). If it fails, it reads the error and throws a specific `HttpRequestException`. | `if (!response.IsSuccessStatusCode) { ... }` |
| **Return Value** | Returns the raw, unprocessed data as a **plain JSON `string`**. | `return await response.Content.ReadAsStringAsync();` |

---

### 📈 2. `BitcoinService.cs` (The Business Logic & Data Processor)

This class acts as the **data orchestrator**. It contains the application's logic for *what* data to get, *how* to process it, and *what* final C# objects to produce.

| Aspect | Description | Key Code Lines |
| :--- | :--- | :--- |
| **Primary Role** | **Processes the raw JSON** into usable C# objects (`List<DailySummary>`). | `var data = JObject.Parse(json);` |
| **Dependency** | It depends on the `ApiClient` to fetch the raw data, using it as a private field (`private readonly ApiClient _apiClient;`). | `string json = await _apiClient.GetAsync(endpoint);` |
| **Request Building** | Calculates the specific API request parameters, such as the `from` and `to` UNIX timestamps for the last 365 days. | `var to = DateTimeOffset.UtcNow.ToUnixTimeSeconds();` |
| **Deserialization/Parsing** | Takes the raw JSON string and converts it into a structured C# object using the **Newtonsoft.Json** library's `JObject.Parse()`. | `var data = JObject.Parse(json);` |
| **Data Transformation** | Manually iterates through the raw JSON arrays (using LINQ to JSON), extracts the price and volume values, and converts the raw UNIX timestamp into a clean `DateTime` object. | `var date = DateTimeOffset.FromUnixTimeMilliseconds(timestamp)...` |
| **Return Value** | Returns a list of clean, strongly-typed C# objects (`List<DailySummary>`) ready for the WPF UI. | `return summaries;` |