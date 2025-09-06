# Story: Macro Deck UI Framework

## Ziel
Ein modernes, erweiterbares und reaktives UI-Framework für Macro Deck, inspiriert von Flutter/SwiftUI, mit Angular als Renderer und C# als deklarative DSL.  
Es muss unabhängig vom restlichen Macro Deck sein, in Plugins nutzbar, DI-kompatibel, testbar und auf Performance ausgerichtet.  

---

## Architektur

### Backend (C#)
- **Library:** `backend/sdk/MacroDeck.SDK.Ui`
- **Oberste Ebene:** `Control`-Klassen
- **Registry:**
  - Beim Start werden alle Controls mit `ExportControl = true` registriert.
  - Registry prüft auf Namenskonflikte → Fehler, wenn 2 mit gleichem Namen registriert.
  - Plugins können eigene Controls registrieren.
- **BaseControl:**
  - Eigenschaften: `Margin`, `Padding`, `Display` (Enum: `Flex`, `None`, `Block` → mappt auf `d-flex`, `d-none`, `d-block`)
  - `CustomClass`, `CustomStyle`
- **ExportControl:**
  - Markiert Controls, die im Angular `<macro-deck-ui-outlet>` gerendert werden können.
- **PluginBuilder:**
  - Option: `ConfigureSettingsControl<T>()` → registriert Settings-UI
  - Actions können optional Controls konfigurieren.
- **SignalR-Verbindung:**
  - Kapselung in Transport-Layer (`IUiTransport`).
  - Settings erlauben: Endpoint, Factory, oder bestehende Connection.
  - Multiplexing per `controlId`.
- **State Management:**
  - Voll reaktiv (`State<T>`, `Computed<T>`).
  - Diff/Patch-System zwischen Controls und Angular.

---

### Frontend (Angular)
- **Library:** `frontend/projects/ui`
- **Outlet Component:**
  ```html
  <macro-deck-ui-outlet control="ExampleControl" [options]="options"></macro-deck-ui-outlet>
  ```
  - Füllt 100% Breite/Höhe des Parents.
  - `options` enthält Transport (SignalR), evtl. auch Theme/Settings.
- **Renderer:**
  - Nutzt moderne Angular Features: `@for`, `@switch`, `@if`.
  - Patch-System für Updates (kein Full Re-Render).
  - Namespaced per `controlId`.
- **Styling:**
  - Bootstrap-first (Buttons, Forms, Grid, Spacing).
  - So wenig Custom-CSS wie möglich.
- **Forms:**
  - Validierbar, Error-Ausgabe als Liste von Labels.
  - Reaktives Binding zwischen C#-State und Angular-Form.
- **Assets:**
  - `Image(key: "...")` → lädt aus Asset Store (`Resources/`).
  - `Image(url: "...")` → direkt im Frontend.

---

## Controls

### Layout
- `VStack` → Flex Column
- `HStack` → Flex Row
- `Panel` (Container mit Background/Border optional)

### Basics
- `Button`
  - Props: `Margin`, `Padding`, `Role` (Primary, Secondary, Danger … = Bootstrap Colors)
  - Events: `onClick`
- `Label`
  - One-way Binding auf State/Text
- `Image`
  - Quelle: `key` (Assets Store) oder `url`

### Forms
- `TextField`
- `TextBox`
- `RadioButtons`
- `Checkbox`
- Alle validierbar, Fehlertexte = `Label[]`

### Progress
- `ProgressBar`
  - Bootstrap `.progress` mit dynamischem Wert

---

## Features

- **Assets Store**
  - Statische Assets im `Resources`-Ordner.
  - Zugriff über Schlüssel → `Image(key: "logo")`.
- **Reaktiv**
  - Änderungen am State triggern Patches → Angular Signals aktualisieren DOM.
- **SignalR**
  - Abstrakter Transport, bestehende Connection kann übergeben werden.
  - Multiplexing über `controlId`.
- **Erweiterbar**
  - Controls als Klassen, vererbbar von `BaseControl`.
  - Plugins können neue Controls registrieren.
- **Unabhängig**
  - Keine direkte Kopplung an Macro Deck Core.
- **DI-Kompatibel**
  - Services wie `IUiHost`, `IUiTransport` injizierbar.
- **Testbar**
  - AST/Patch Tests (Golden Files).
  - Angular-Renderer testbar über Storybook/Playwright.
- **Performance**
  - Diff/Patch statt Full Re-Render.
  - Bootstrap für Layout → keine eigene Layoutengine.
  - Throttling/Debounce bei lauten Events (z. B. Slider).
- **Cross-Plugin**
  - Plugins sprechen nur SignalR/REST/gRPC → UI funktioniert auch remote.

---

## Ausarbeitung der offenen Punkte

### 1) Navigation / Routing

**Ziel:** Mehrseitige Views (Screens) und modale Dialoge steuern – je `controlId` isoliert, DI-kompatibel, patch-basiert.

**Konzepte**
- `INavigationService` (C#) je `controlId` (scoped).
- Stack-basierte Navigation: `Push(screen)`, `Pop()`, `Replace(screen)`, `Reset(root)`.
- Screens sind **Controls** (AST-Subtrees) mit stabilem `screenKey`.
- Angular rendert die oberste Screen-Ebene; Transitions optional.

**C#-Interfaces**
```csharp
public interface INavigationService
{
    string ControlId { get; }
    void Push(Control screen, string? screenKey = null);
    void Replace(Control screen, string? screenKey = null);
    void Pop();
    void Reset(Control rootScreen, string? screenKey = null);
    IReadOnlyList<string> StackKeys { get; }
}
```

**DI-Registrierung**
- `INavigationService` ist **scoped per Control/View**.
- `IUiHost.OpenView(...)` erstellt einen Scope, registriert `INavigationService` keyed mit `controlId`.

**Angular**
- Renderer erhält `navigationStack` als Teil des AST (Liste von Screen-Nodes).
- `<macro-deck-ui-outlet>` rendert nur den **Top**-Screen, optional `@switch` auf Transitionen.
- Dialoge: `NavigationService.OpenDialog(control)` → separater Overlay-Container, eigener Stack.

**Warum keyed DI?**
- Mehrere offene Views sollen ihre Navigation entkoppelt steuern können. Key = `controlId`.

---

### 2) Vollständige DI-Kompatibilität

**Backend**
- Alle Services (`IUiHost`, `IUiTransport`, `INavigationService`, `IAssetsStore`, `IValidationService`) über DI.
- Controls bekommen Services via Konstruktor-Injection (über Factory/Activator).
- Pro View eigener **DI-Scope**:
  - Lebensdauer: vom `OpenView` bis `CloseView`.
  - Vorteil: Events/State/Effekte liegen sauber im Scope; Memory-Leaks werden verhindert.

**Frontend**
- Angular: stateless Renderer + `RendererService` als Injectable.
- `UiStore` (Signals) per `viewId` namespaced.
- Transport-Client (`UiTransportClient`) als Service; via `options` im Outlet konfigurierbar oder global per Provider.

---

### 3) Virtualisierung für lange Listen

**Ansatz**
- Standardmäßig aktiv für `ListView`/`ForEach`-ähnliche Controls.
- Serverseitig liefert das Control **Datenfenster** (windowing): `offset`, `limit`, `total`.
- Angular rendert virtuell (CDK Virtual Scroll **oder** eigene schlanke Directive).
- Patches beschreiben:
  - `windowChanged(offset, limit)`
  - `itemsInserted/Removed/Updated` relativ zur Window-Basis.
- Smooth Scrolling + Anchor Recycling.

**Pseudocode AST**
```json
{
  "type":"ListView",
  "id":"orders",
  "props":{"itemHeight":32,"total":50000,"window":{"offset":0,"limit":100}},
  "children":[ /* 100 sichtbare Child-Nodes */ ]
}
```

**Events**
- Angular → C#: `onWindowRequest { controlId:"...", id:"orders", offset, limit }`
- C# aktualisiert Window-State + liefert Patch mit neuen Items.

---

### 4) Hot Reload für Controls (Plugin-Entwicklung)

**Ziel:** Änderungen an Control-Klassen im Plugin ohne Neustart sichtbar machen – minimal invasiv.

**Strategie**
- **Dev-Modus** (Feature-Flag): `UiDevOptions.EnableHotReload = true`.
- **ControlFactory** löst Controls **nicht** direkt via `new`, sondern über eine **ReloadableProxyFactory**:
  - Hält `AssemblyLoadContext` pro Plugin im Dev-Modus.
  - Beobachtet (FileWatcher) die DLL des Plugins.
  - Bei Änderung: Unload alter ALC → neu laden → **Re-Instantiieren** betroffener Controls im nächsten Render-Tick.
- **State Preservation**
  - Versuch, `SerializableState` via Interface zu extrahieren:
    ```csharp
    public interface IHotReloadable
    {
        object? CaptureState();
        void RestoreState(object? snapshot);
    }
    ```
  - Beim Reload: `CaptureState()` → neue Instanz → `RestoreState(snapshot)`.
- **Sicherheit**
  - Kein automatisches Reload in Production.
  - Reload ist best-effort, im Fehlerfall fällt es auf Full Rebuild des Screens zurück.

**Angular**
- Keine Änderungen nötig – erhält einfach neue AST/Patches.

---

### 5) Beispiel im `ExamplePlugin`

**Struktur**
- `plugins/ExamplePlugin/`
  - `ExampleCounterControl.cs` (exportiert)
  - `ExampleSettingsControl.cs` (optional)
  - `Resources/logo.png`
  - `ExamplePlugin.cs` (Registrierung)

**ExampleCounterControl.cs**
```csharp
[ExportControl] // Marker-Attribut
public sealed class ExampleCounterControl : Control
{
    private readonly IAssetsStore _assets;
    private readonly INavigationService _nav;

    private readonly State<int> _count = State.Of(0);

    public ExampleCounterControl(IAssetsStore assets, INavigationService nav)
    {
        _assets = assets;
        _nav = nav;
    }

    public override Node Build()
    {
        return new VStack(padding: 16).Children(
            new Image(key: "logo"),
            new Label(() => $"Count: {_count.Value}"),
            new HStack(spacing: 8).Children(
                new Button("Increment").Role(ButtonRole.Primary).OnClick(() => _count.Value++),
                new Button("Details").OnClick(OpenDetails)
            ),
            new ProgressBar(() => (_count.Value % 100) / 100.0)
        );
    }

    private void OpenDetails()
    {
        _nav.Push(new DetailsScreenControl(_count));
    }
}
```

**ExamplePlugin.cs**
```csharp
public sealed class ExamplePlugin : IMacroDeckPlugin
{
    public void Configure(IMacroDeckPluginBuilder builder)
    {
        builder
            .RegisterControlsFromAssembly(typeof(ExampleCounterControl).Assembly)
            .ConfigureSettingsControl<ExampleSettingsControl>(); // optional
    }
}
```

**Angular Demo-Einbindung**
```html
<!-- frontend/app.component.html -->
<macro-deck-ui-outlet control="ExampleCounterControl" [options]="options"></macro-deck-ui-outlet>
```

**Options (TS)**
```ts
export interface MacroDeckUiOptions {
  viewId?: string; // optional, wird sonst generiert
  transport?: UiTransportClient; // gekapselte SignalR-Verbindung oder Factory
  theme?: 'auto' | 'light' | 'dark';
}
```

---

## Schnittstellen & Skeletons

### C# – Transport & Host
```csharp
public interface IUiHost
{
    IUiView OpenView(string controlName, MacroDeckUiOptions? options = null);
    void ApplyPatches(string viewId, IReadOnlyList<Patch> patches);
    void CloseView(string viewId, string? reason = null);
}

public interface IUiView : IAsyncDisposable
{
    string ViewId { get; }
    Task SendEventAsync(string nodeId, string @event, object? payload = null, CancellationToken ct = default);
}

public sealed class MacroDeckUiOptions
{
    public string? ViewId { get; init; }
    public UiTransportSettings? TransportSettings { get; init; }
    public IDictionary<string, object?>? Parameters { get; init; }
}
```

### C# – DI Registration
```csharp
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddMacroDeckUi(this IServiceCollection services, Action<UiTransportSettings>? cfg = null)
    {
        services.AddSingleton<IUiHost, UiHost>();
        services.AddSingleton<IUiTransportFactory, SignalRTransportFactory>();
        services.AddSingleton<IControlRegistry, ControlRegistry>();
        services.AddSingleton<IAssetsStore, ResourcesAssetsStore>();
        services.AddSingleton<IValidationService, ValidationService>();

        services.AddScoped<INavigationService, NavigationService>(); // per view scope

        if (cfg is not null)
        {
            services.Configure(cfg);
        }
        return services;
    }
}
```

### Angular – Outlet Skeleton
```ts
// frontend/projects/ui/src/lib/outlet/outlet.component.ts
import { Component, ElementRef, Input, OnDestroy, OnInit, effect, inject } from '@angular/core';
import { RendererService } from '../renderer/renderer.service';
import { MacroDeckUiOptions } from '../types';

@Component({
  selector: 'macro-deck-ui-outlet',
  template: \`
    <div class="w-100 h-100 d-flex flex-column">
      <ui-root [viewId]="viewId"></ui-root>
    </div>
  \`,
  styles: [':host{display:block; width:100%; height:100%;}']
})
export class MacroDeckUiOutletComponent implements OnInit, OnDestroy {
  @Input() control!: string;
  @Input() options?: MacroDeckUiOptions;

  private readonly renderer = inject(RendererService);
  viewId!: string;

  ngOnInit(): void {
    this.viewId = this.options?.viewId ?? crypto.randomUUID();
    this.renderer.attach(this.viewId, this.control, this.options);
  }

  ngOnDestroy(): void {
    this.renderer.detach(this.viewId);
  }
}
```

### Angular – RendererService (Skizze)
```ts
export class RendererService {
  attach(viewId: string, control: string, options?: MacroDeckUiOptions) {
    // 1) Transport verbinden/abonnieren (ready handshake)
    // 2) openView(control) an Backend senden
    // 3) AST/patches in Signals-Store pro viewId schreiben
  }
  detach(viewId: string) {
    // closeView → Store/Subscriptions/Treiber aufräumen
  }
}
```

---

## Forms & Validation

**Ziele**
- Serverseitige Validierung (C#) + clientseitige Bootstrap-Darstellung.
- Error-Liste einfach als `Label[]` darstellbar.

**C#**
- `IValidationService.Validate(model)` → `ValidationResult { IsValid, Errors[] }`
- Form-Controls senden `onChange` → Server validiert → Patch setzt `errors`-Prop am Control.

**AST-Beispiel**
```json
{
  "type":"TextField",
  "id":"email",
  "props":{
    "label":"Email",
    "value":"foo@bar",
    "errors":["Bitte gültige Email angeben."]
  }
}
```

**Angular**
- `@if(node.props.errors?.length) { <div class="invalid-feedback">@for(err of node.props.errors) { {{err}} }</div> }`
- Bootstrap-Klassen `is-invalid` / `is-valid` werden aus Props abgeleitet.

---

## Testbarkeit

- **Backend**
  - Snapshot-Tests: AST als Golden Files (JSON).
  - Patch-Tests: Diff-Engine auf deterministische Patches prüfen.
  - Navigation-Tests: Stack-Manipulationen isoliert testen.
- **Frontend**
  - Component-Tests: Renderer erzeugt erwartete DOM-Struktur für gegebenen AST.
  - E2E: Playwright – Events roundtrip, Close/Destroy, Virtual Scroll.
- **Contract**
  - JSON-Schema-Validation für beide Richtungen (AST & Patches).

---

## Security & Robustheit

- Keine HTML-Injection (Text ist plain).
- Größenlimits für Patches/Events, Throttling.
- Idempotentes `closeView`.
- Timeouts & Reconnect-Backoff im Transport.

---

## Roadmap (kurz)

1) Contract v1 (AST/Patch/Event Schema + Codegen)
2) Renderer MVP (Panel/VStack/HStack/Text/Button/Image)
3) Forms + Validation
4) Navigation + Dialogs
5) Virtualization
6) Hot Reload (Dev only)
7) ExamplePlugin polish + Docs


---

## Plugins: Transport, Ownership & Registry-Lifecycle

### Zielbild
Frontend ist **nur** mit dem **Server** verbunden. Plugins sind **eigene Services**, die per **WebSocket** mit **Protobuf**-Nachrichten mit dem Server sprechen. Der Server ist Broker/Orchestrator für UI:
```
Frontend  ←→  Server (UI Broker)  ←→  Plugin-Service (WebSocket+Protobuf)
```

### Ownership & Namespacing
- Jedes Control hat einen **Owner** (`ownerType: Core|Plugin`, `ownerId: pluginId`).
- `controlName` ist **global eindeutig** (Server prüft Kollisionen).
- `viewId` wird **serverseitig** erzeugt; Events/Patches tragen immer `viewId` **und** `ownerId`.
- Asset-Keys werden namespaced: `pluginId/path/to.png`.

### Registry-Workflow
1. **Plugin-Connect** (WebSocket/Protobuf):
   - Server legt `PluginSession { pluginId, capabilities, version }` an.
2. **RegisterControls** (vom Plugin):
   - Server validiert, schreibt in `ControlRegistry` (Owner=Plugin).
   - Kollision → `RegisterRejected` (inkl. Konfliktliste).
3. **OpenView** (vom Frontend/Server):
   - Wenn `owner=Plugin`: Server sendet `OpenView` an Plugin mit `viewId`, `controlName`, `parameters`.
   - Plugin baut AST (Protobuf), sendet `OpenViewResult` mit Root-Node an Server → Server → Frontend.
4. **Patches/Events**:
   - **Frontend Event** → Server → Plugin (`UiEvent`).
   - **Plugin Patch** → Server → Frontend (`ApplyPatches`).
5. **Plugin-Disconnect**:
   - Server **schließt** alle offenen Views dieses Owners (`closeView(reason="plugin_disconnected")`).
   - Controls werden **aus Registry entfernt**.
   - Optional: Persistenter Fehlerbildschirm je betroffener View.

### Lebenszyklen & Heartbeat
- **Heartbeat**: Plugin sendet regelmäßig `Ping`; Server misst Latenz, Timeouts schließen Session.
- **Backpressure**: Server throttelt Patch-Rate pro `pluginId` & `viewId`.
- **Reconnections**: Session-Recovery optional via `resumeToken`; Registry wird **nicht** automatisch restauriert → Plugin muss erneut `RegisterControls` senden.

### Sicherheit & Verträge
- **Protokoll-Version**: beide Seiten senden `protocolVersion`. Server kann „incompatible“ ablehnen.
- **Auth**: WebSocket mit Bearer (z. B. mTLS/Access-Token). `pluginId` wird serverseitig eindeutig zugeordnet.
- **Sanitizing**: Server validiert AST/Props gegen JSON/Proto-Schema (z. B. keine fremden Events/Props).

### Protobuf (Sketch)
```proto
syntax = "proto3";
package macrodeck.ui;

message Connect {
  string plugin_id = 1;
  string protocol_version = 2;
  repeated string capabilities = 3; // e.g., "list_virtualization"
}

message RegisterControls {
  repeated ControlDescriptor controls = 1;
}
message ControlDescriptor {
  string name = 1;           // global name
  string display_name = 2;   // optional
  string category = 3;       // optional
  repeated string tags = 4;  // optional
}

message RegisterAck {
  bool ok = 1;
  repeated string conflicts = 2;
  string message = 3;
}

message OpenViewRequest {
  string view_id = 1;
  string control_name = 2;
  map<string, string> parameters = 3;
}
message OpenViewResult {
  string view_id = 1;
  AstNode root = 2; // serialized AST (proto)
}

message UiEvent {
  string view_id = 1;
  string node_id = 2;
  string event = 3;
  bytes payload = 4; // JSON or proto-encoded
}

message ApplyPatches {
  string view_id = 1;
  repeated Patch patches = 2;
}

// ... Patch/AstNode messages analog zum JSON-Schema
```

### Server-Datenmodell (vereinfacht)
- `PluginSession(pluginId, connectedAt, lastPingAt, version, capabilities)`
- `ControlRegistry(controlName → { ownerType, ownerId, descriptor })`
- `OpenViews(viewId → { ownerId, controlName, state })`

### Sequenzen (vereinfacht)

**A) Frontend öffnet ein Plugin-Control**
```
FE ──open(controlName, params)──▶ Server
Server ──OpenViewRequest(viewId, controlName, params)──▶ Plugin
Plugin ──OpenViewResult(viewId, rootAst)──▶ Server
Server ──openView(viewId, rootJson)──▶ FE
```

**B) User-Event**
```
FE ──uiEvent(viewId, nodeId, event, payload)──▶ Server
Server ──UiEvent(viewId, nodeId, event, payload)──▶ Plugin
Plugin ──ApplyPatches(viewId, patches)──▶ Server
Server ──applyPatches(viewId, patchesJson)──▶ FE
```

**C) Plugin-Disconnect**
```
Plugin ──(socket closed)──▶ Server
Server: remove Controls(ownerId), for each viewId(ownerId): closeView(reason), notify FE
```

### Fehler-/Recovery-Strategien
- **Orphan Views**: Wenn Plugin weg ist, zeigt FE einen schlichten Error-Screen („Plugin getrennt“).
- **Slow Plugins**: Server setzt `eventAckTimeout`; nach Timeout optional Retry/Nack.
- **Oversized Patches**: Server chunked oder lehnt ab (413) mit Hinweis auf Virtualisierung.

### DI-Scopes pro Plugin
- Server erstellt pro `PluginSession` einen DI-Scope (`IPluginContext`), darin:
  - `IAssetsStore` (namespaced auf `pluginId`)
  - `ILog` mit Plugin-Kontext
  - `IUiSerialization` (Proto/JSON Adapter)
  - **Pro View** ein weiterer Child-Scope (für Navigation/State).

### Beispiel-Server-Code (C# Skizze)
```csharp
public sealed class PluginUiBroker
{
    private readonly IControlRegistry _registry;
    private readonly IViewManager _views;
    private readonly ILogger _log;

    public async Task OnRegisterControlsAsync(string pluginId, IEnumerable<ControlDescriptor> controls)
    {
        var conflicts = new List<string>();
        foreach (var c in controls)
        {
            var ok = _registry.TryAdd(c.Name, ownerType: OwnerType.Plugin, ownerId: pluginId, c, out var conflict);
            if (!ok && conflict is not null) conflicts.Add(conflict);
        }
        if (conflicts.Count > 0) throw new RegistryConflictException(conflicts);
    }

    public async Task OpenPluginViewAsync(string controlName, IDictionary<string, string> parameters)
    {
        var owner = _registry.ResolveOwner(controlName) ?? throw new InvalidOperationException("Unknown control");
        var viewId = Guid.NewGuid().ToString("N");
        await _plugins.SendAsync(owner.PluginId, new OpenViewRequest { view_id = viewId, control_name = controlName, parameters = { parameters } });
        _views.Add(viewId, owner.PluginId, controlName);
    }

    public Task OnOpenViewResultAsync(OpenViewResult msg)
    {
        var jsonAst = ProtoToJson(msg.root);
        return _frontend.SendOpenViewAsync(msg.view_id, jsonAst);
    }

    public Task OnUiEventFromFrontendAsync(string viewId, string nodeId, string ev, object payload)
    {
        var view = _views.Get(viewId);
        return _plugins.SendAsync(view.PluginId, new UiEvent { view_id = viewId, node_id = nodeId, event = ev, payload = Serialize(payload) });
    }

    public Task OnApplyPatchesFromPluginAsync(ApplyPatches ap)
        => _frontend.SendApplyPatchesAsync(ap.view_id, ProtoToJson(ap.patches));
}
```

### Beispiel: Plugin-Registrierung (Pseudocode)
```csharp
// Plugin-Seite
var socket = new WebSocketClient(endpoint, token);
await socket.Send(new Connect { plugin_id = "example", protocol_version = "1.0" });
await socket.Send(new RegisterControls {
  controls = { new ControlDescriptor { name = "ExampleCounterControl", display_name = "Counter" } }
});
```

---

## Auswirkungen auf bestehende Bereiche

- **Navigation**: funktioniert unverändert; `INavigationService` bleibt **serverseitig** und schickt die resultierenden AST/Patches über das Plugin.
- **Virtualisierung**: Window-Requests laufen Frontend → Server → Plugin; Plugin liefert Datenfenster als AST-Chunk.
- **Hot Reload**: Bei Dev-Plugins kann der Server den Plugin-Prozess restarten oder ALC neu laden; Registry wird danach erneut aufgebaut.

---

## Checkliste (Server)
- [ ] Owner-Feld im `ControlRegistry`
- [ ] PluginSession mit Heartbeat/Timeout
- [ ] Protobuf-Nachrichten implementiert (Connect/Register/OpenView/UiEvent/ApplyPatches/Close)
- [ ] View-Teardown bei Disconnect
- [ ] Asset-Namespace `pluginId/*`
- [ ] Rate-Limits pro Plugin & View
- [ ] Schema-Validation der AST/Patches
