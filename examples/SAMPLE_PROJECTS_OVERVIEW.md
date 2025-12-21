# OpenCode Sample Projects Overview

This document provides a comprehensive guide to the 6 new sample projects scaffolded in the `/src/opencode-dotnet/examples/` directory.

## Project Summary

| Project | Type | Purpose | Framework | Status |
|---------|------|---------|-----------|--------|
| **Shared.Components** | Class Library | Reusable UI components | Razor Class Library | Template |
| **AgUi.Chat.BlazorServer** | Web App | Minimal chat UI | Blazor Server | Template |
| **AgUi.IDE.BlazorServer** | Web App | Full IDE | Blazor Server | Template |
| **AgUi.Chat.BlazorWasm** | Web App | Chat with client/server split | Blazor WASM | Template |
| **AgUi.IDE.BlazorWasm** | Web App | IDE with client/server split | Blazor WASM | Template |
| **AgUi.IDE.Desktop** | Desktop App | Native desktop IDE | MAUI + Blazor Hybrid | Template |

## Project Descriptions

### 1. Shared.Components

**Location**: `/src/opencode-dotnet/examples/Shared.Components/`

**Type**: Razor Class Library (.NET 9.0)

**Purpose**: Reusable UI component library for OpenCode Blazor applications

**Key Components**:
- `IDEView.razor` - Integrated development environment layout with resizable panels
- `ChatPanel.razor` - AI chat interface with message history and markdown rendering
- `DiffPanel.razor` - File difference viewer with syntax highlighting
- `FilesPanel.razor` - File explorer with tree view
- `TerminalPanel.razor` - Terminal emulator interface

**Usage**: Reference in web and desktop applications to use pre-built IDE components

**Dependencies**:
- Microsoft.AspNetCore.Components.Web
- LionFire.OpenCode.Blazor

**TODO Items**:
- Component implementation (5 main sections)
- Styling & theming (light/dark modes)
- Testing & accessibility
- 40+ implementation tasks

**Key Files**:
- `/src/opencode-dotnet/examples/Shared.Components/README.md`
- `/src/opencode-dotnet/examples/Shared.Components/TODO.md`

---

### 2. AgUi.Chat.BlazorServer

**Location**: `/src/opencode-dotnet/examples/AgUi.Chat.BlazorServer/`

**Type**: Blazor Server Application (.NET 9.0)

**Purpose**: Minimal chat interface with AI agents using Blazor Server and SignalR

**Architecture**:
- Single application with interactive server-side rendering
- Real-time communication via Blazor's built-in SignalR
- Integrated with OpenCode Serve API

**Key Files**:
- `Program.cs` - Service registration and middleware configuration
- `Components/App.razor` - Root layout
- `Components/Routes.razor` - Router setup
- `Components/Pages/Home.razor` - Chat page with TODO for ChatPanel integration

**Dependencies**:
- Microsoft.AspNetCore.Components.Web
- LionFire.OpenCode.Serve
- LionFire.OpenCode.Blazor

**TODO Items**:
- OpenCode API client configuration
- Chat UI implementation
- Message streaming support
- Conversation history
- Code syntax highlighting
- 30+ implementation tasks

**Key Files**:
- `/src/opencode-dotnet/examples/AgUi.Chat.BlazorServer/README.md`
- `/src/opencode-dotnet/examples/AgUi.Chat.BlazorServer/TODO.md`

---

### 3. AgUi.IDE.BlazorServer

**Location**: `/src/opencode-dotnet/examples/AgUi.IDE.BlazorServer/`

**Type**: Blazor Server Application (.NET 9.0)

**Purpose**: Full-featured IDE with file explorer, editor, chat, and terminal

**Architecture**:
- Multi-panel IDE layout using Shared.Components
- Server-side rendering with Blazor SignalR
- Project file management via OpenCode API
- Real-time updates for file changes

**Key Files**:
- `Program.cs` - Service registration with Shared.Components
- `Components/Pages/IDE.razor` - Main IDE page using IDEView component
- `Components/App.razor` - Root layout

**Dependencies**:
- LionFire.OpenCode.Serve
- LionFire.OpenCode.Blazor
- Shared.Components

**TODO Items**:
- IDE layout implementation
- Project loading and file management
- File watching and real-time updates
- Chat integration with context
- Terminal implementation
- VS Code-like keyboard shortcuts
- 45+ implementation tasks

**Key Files**:
- `/src/opencode-dotnet/examples/AgUi.IDE.BlazorServer/README.md`
- `/src/opencode-dotnet/examples/AgUi.IDE.BlazorServer/TODO.md`

---

### 4. AgUi.Chat.BlazorWasm

**Location**: `/src/opencode-dotnet/examples/AgUi.Chat.BlazorWasm/`

**Type**: Blazor WebAssembly with Server Backend

**Purpose**: Chat application split between client (WASM) and server (ASP.NET Core)

**Architecture**:
- **Client**: WebAssembly application running in browser
- **Server**: ASP.NET Core backend hosting WASM app and providing APIs
- **Shared**: Common models and DTOs

**Project Structure**:
```
AgUi.Chat.BlazorWasm/
  ├── Client/
  │   ├── Program.cs
  │   ├── App.razor
  │   ├── Routes.razor
  │   └── Pages/Home.razor
  ├── Server/
  │   ├── Program.cs
  │   ├── App.razor
  │   └── Controllers/
  └── Shared/
      └── (Models and DTOs)
```

**Key Features**:
- Client-side rendering reduces server load
- RESTful API between client and server
- Fast initial page load (after WASM download)
- Better offline capabilities

**Dependencies**:
- Microsoft.AspNetCore.Components.WebAssembly
- LionFire.OpenCode.Serve (Server project)
- LionFire.OpenCode.Blazor (Client project)

**TODO Items**:
- Server API endpoints for chat
- Client-server communication
- Shared message DTOs
- Streaming response support
- Error handling and resilience
- Testing and deployment
- 50+ implementation tasks

**Key Files**:
- `/src/opencode-dotnet/examples/AgUi.Chat.BlazorWasm/README.md`
- `/src/opencode-dotnet/examples/AgUi.Chat.BlazorWasm/TODO.md`

---

### 5. AgUi.IDE.BlazorWasm

**Location**: `/src/opencode-dotnet/examples/AgUi.IDE.BlazorWasm/`

**Type**: Blazor WebAssembly with Server Backend

**Purpose**: Full-featured IDE split between client (WASM) and server (ASP.NET Core)

**Architecture**:
- **Client**: WebAssembly IDE with Shared.Components
- **Server**: ASP.NET Core backend with project/file API endpoints
- **Shared**: Models for client-server communication

**Project Structure**:
```
AgUi.IDE.BlazorWasm/
  ├── Client/
  │   ├── Program.cs
  │   ├── App.razor
  │   ├── Routes.razor
  │   └── Pages/IDE.razor (uses Shared.Components)
  ├── Server/
  │   ├── Program.cs
  │   ├── App.razor
  │   └── Controllers/
  └── Shared/
      └── (File/Project models)
```

**Key Features**:
- Multi-panel IDE using Shared.Components
- Project file browser and management
- Code editor with syntax highlighting
- AI chat assistant integration
- Terminal emulator
- Diff viewer
- Resizable panel layout

**Dependencies**:
- Microsoft.AspNetCore.Components.WebAssembly
- LionFire.OpenCode.Serve (Server)
- LionFire.OpenCode.Blazor (Client)
- Shared.Components

**TODO Items**:
- Server API for projects and files
- Client-side project management
- File watching and real-time updates
- Code editor integration
- Chat with context support
- Terminal sessions
- Keyboard shortcuts
- 60+ implementation tasks

**Key Files**:
- `/src/opencode-dotnet/examples/AgUi.IDE.BlazorWasm/README.md`
- `/src/opencode-dotnet/examples/AgUi.IDE.BlazorWasm/TODO.md`

---

### 6. AgUi.IDE.Desktop

**Location**: `/src/opencode-dotnet/examples/AgUi.IDE.Desktop/`

**Type**: MAUI Blazor Hybrid Application (.NET 9.0)

**Purpose**: Native desktop IDE for Windows and macOS using MAUI + Blazor

**Architecture**:
- **Native Shell**: XAML-based MAUI UI for window management and navigation
- **Blazor Components**: Shared.Components rendered in WebView
- **Process Management**: Native process control for OpenCode server
- **Platform Integration**: Windows and macOS-specific APIs

**Project Structure**:
```
AgUi.IDE.Desktop/
  ├── MauiProgram.cs
  ├── App.xaml/cs
  ├── AppShell.xaml/cs
  ├── MainPage.xaml/cs
  ├── Components/Routes.razor
  ├── Services/OpenCodeProcessManager.cs
  ├── Platforms/
  │   ├── Windows/Platform.cs
  │   └── MacCatalyst/Platform.cs
  └── [All other files]
```

**Key Features**:
- Native desktop window management
- Embedded Blazor IDE components
- OpenCode process lifecycle management
- Cross-platform support (Windows, macOS)
- System integration (file browser, terminal)
- Native XAML UI + WebView Blazor components

**Target Frameworks**:
- `net9.0-windows` - Windows 10 and newer
- `net9.0-maccatalyst` - macOS 13.1 and newer

**Dependencies**:
- Microsoft.Maui.Sdk (MAUI framework)
- CommunityToolkit.Mvvm (MVVM support)
- LionFire.OpenCode.Serve
- LionFire.OpenCode.Blazor
- Shared.Components

**Key Services**:
- `OpenCodeProcessManager` - Start/stop/monitor OpenCode processes
- Project management services
- File management services

**TODO Items**:
- MAUI window configuration
- Blazor WebView integration
- OpenCodeProcessManager implementation
- Platform-specific file handling
- Terminal integration
- Settings and preferences
- Build and distribution
- 60+ implementation tasks

**Key Files**:
- `/src/opencode-dotnet/examples/AgUi.IDE.Desktop/README.md`
- `/src/opencode-dotnet/examples/AgUi.IDE.Desktop/TODO.md`

---

## Architecture Comparison

### Rendering Models

```
Blazor Server (AgUi.Chat.BlazorServer, AgUi.IDE.BlazorServer)
├── Rendering: Server-side via SignalR
├── Interactivity: Interactive Server
├── Use Case: Real-time apps with high interactivity
└── Scalability: Requires persistent SignalR connections

Blazor WebAssembly (AgUi.Chat.BlazorWasm.Client, AgUi.IDE.BlazorWasm.Client)
├── Rendering: Client-side (downloaded as WASM)
├── Interactivity: Full client-side
├── Use Case: Offline-capable, scalable apps
└── Scalability: Better (stateless server)

MAUI + Blazor Hybrid (AgUi.IDE.Desktop)
├── Rendering: WebView-hosted Blazor components
├── Interactivity: Client-side + native XAML
├── Use Case: Desktop apps with web UI
└── Scalability: Single user per instance
```

### Component Sharing Strategy

```
Shared.Components (Razor Class Library)
├── Used by: All web and desktop projects
├── Provides: IDEView, ChatPanel, DiffPanel, FilesPanel, TerminalPanel
├── Benefits: Code reuse across all UI platforms
└── Integration: Referenced as project dependency
```

### API Architecture

```
OpenCode Serve API
├── Used by: All projects for backend operations
├── Provides: File operations, chat, code generation
├── Communication: HTTP/REST or SignalR
└── Integration: DI in Program.cs

OpenCode Blazor Library
├── Used by: All Blazor projects
├── Provides: Blazor-specific OpenCode integration
├── Features: Components, services, utilities
└── Integration: Project reference in .csproj
```

---

## Getting Started with Each Project

### Building All Projects

```bash
cd /src/opencode-dotnet/examples

# Restore all dependencies
dotnet restore

# Build all projects
dotnet build

# Build specific project
dotnet build Shared.Components/Shared.Components.csproj
dotnet build AgUi.Chat.BlazorServer/AgUi.Chat.BlazorServer.csproj
```

### Running Individual Projects

```bash
# Blazor Server (runs on https://localhost:5001)
cd AgUi.Chat.BlazorServer && dotnet run

# Blazor Server IDE
cd AgUi.IDE.BlazorServer && dotnet run

# Blazor WebAssembly Chat (server runs on https://localhost:5001)
cd AgUi.Chat.BlazorWasm/Server && dotnet run

# Blazor WebAssembly IDE
cd AgUi.IDE.BlazorWasm/Server && dotnet run

# MAUI Desktop (Windows)
cd AgUi.IDE.Desktop && dotnet run -f net9.0-windows

# MAUI Desktop (macOS)
cd AgUi.IDE.Desktop && dotnet run -f net9.0-maccatalyst
```

---

## Key Features Across All Projects

### Common Components (from Shared.Components)

1. **IDEView**
   - Multi-panel IDE layout
   - Resizable panels with splitters
   - Component composition

2. **ChatPanel**
   - Message history display
   - Streaming response support
   - Markdown rendering
   - Code syntax highlighting

3. **DiffPanel**
   - File difference viewer
   - Side-by-side or unified view
   - Syntax highlighting for diffs

4. **FilesPanel**
   - Project file tree
   - File icons by type
   - File search/filtering
   - Status indicators

5. **TerminalPanel**
   - Terminal emulator interface
   - ANSI color support
   - Command history
   - Copy/paste functionality

### OpenCode Integration

All projects integrate with OpenCode Serve API for:
- File operations and management
- Code generation via AI
- Chat with context awareness
- Terminal execution

---

## TODO Management

Each project includes a comprehensive TODO.md with:
- Setup and configuration tasks
- Feature implementation checklist
- UI/UX tasks
- Testing and debugging
- Deployment preparation

Use these TODOs as a guide for implementation priority and progress tracking.

---

## Project Dependencies

### Direct Dependencies

```
Shared.Components
├── LionFire.OpenCode.Blazor
└── Microsoft.AspNetCore.Components.Web

AgUi.Chat.BlazorServer
├── LionFire.OpenCode.Serve
├── LionFire.OpenCode.Blazor
└── Microsoft.AspNetCore.Components.Web

AgUi.IDE.BlazorServer
├── LionFire.OpenCode.Serve
├── LionFire.OpenCode.Blazor
├── Shared.Components
└── Microsoft.AspNetCore.Components.Web

AgUi.Chat.BlazorWasm (Client)
├── LionFire.OpenCode.Blazor
└── Microsoft.AspNetCore.Components.WebAssembly

AgUi.Chat.BlazorWasm (Server)
├── LionFire.OpenCode.Serve
├── Client project
└── Shared project

AgUi.IDE.BlazorWasm (Client)
├── LionFire.OpenCode.Blazor
├── Shared.Components
└── Microsoft.AspNetCore.Components.WebAssembly

AgUi.IDE.BlazorWasm (Server)
├── LionFire.OpenCode.Serve
├── Shared.Components
├── Client project
└── Shared project

AgUi.IDE.Desktop
├── LionFire.OpenCode.Serve
├── LionFire.OpenCode.Blazor
├── Shared.Components
├── Microsoft.Maui.Sdk
└── CommunityToolkit.Mvvm
```

---

## Framework Versions

All projects target **NET 9.0**:
- Shared.Components: net9.0
- Blazor Server projects: net9.0
- Blazor WASM projects: net9.0
- MAUI Desktop: net9.0-windows, net9.0-maccatalyst

---

## File Structure Summary

```
/src/opencode-dotnet/examples/
├── Shared.Components/               (Component Library)
│   ├── Shared.Components.csproj
│   ├── Components/
│   │   ├── IDEView.razor
│   │   ├── ChatPanel.razor
│   │   ├── DiffPanel.razor
│   │   ├── FilesPanel.razor
│   │   └── TerminalPanel.razor
│   ├── README.md
│   └── TODO.md
│
├── AgUi.Chat.BlazorServer/          (Blazor Server - Chat)
│   ├── AgUi.Chat.BlazorServer.csproj
│   ├── Program.cs
│   ├── Components/
│   │   ├── App.razor
│   │   ├── Routes.razor
│   │   └── Pages/Home.razor
│   ├── README.md
│   └── TODO.md
│
├── AgUi.IDE.BlazorServer/           (Blazor Server - IDE)
│   ├── AgUi.IDE.BlazorServer.csproj
│   ├── Program.cs
│   ├── Components/
│   │   ├── App.razor
│   │   ├── Routes.razor
│   │   └── Pages/IDE.razor
│   ├── README.md
│   └── TODO.md
│
├── AgUi.Chat.BlazorWasm/            (Blazor WASM - Chat)
│   ├── Client/
│   │   ├── AgUi.Chat.BlazorWasm.Client.csproj
│   │   ├── Program.cs
│   │   ├── App.razor
│   │   ├── Routes.razor
│   │   └── Pages/Home.razor
│   ├── Server/
│   │   ├── AgUi.Chat.BlazorWasm.Server.csproj
│   │   ├── Program.cs
│   │   ├── App.razor
│   │   └── Controllers/
│   ├── Shared/
│   │   └── AgUi.Chat.BlazorWasm.Shared.csproj
│   ├── README.md
│   └── TODO.md
│
├── AgUi.IDE.BlazorWasm/             (Blazor WASM - IDE)
│   ├── Client/
│   │   ├── AgUi.IDE.BlazorWasm.Client.csproj
│   │   ├── Program.cs
│   │   ├── App.razor
│   │   ├── Routes.razor
│   │   └── Pages/IDE.razor
│   ├── Server/
│   │   ├── AgUi.IDE.BlazorWasm.Server.csproj
│   │   ├── Program.cs
│   │   ├── App.razor
│   │   └── Controllers/
│   ├── Shared/
│   │   └── AgUi.IDE.BlazorWasm.Shared.csproj
│   ├── README.md
│   └── TODO.md
│
├── AgUi.IDE.Desktop/                (MAUI + Blazor Hybrid)
│   ├── AgUi.IDE.Desktop.csproj
│   ├── MauiProgram.cs
│   ├── App.xaml/cs
│   ├── AppShell.xaml/cs
│   ├── MainPage.xaml/cs
│   ├── Components/Routes.razor
│   ├── Services/OpenCodeProcessManager.cs
│   ├── Platforms/
│   │   ├── Windows/Platform.cs
│   │   └── MacCatalyst/Platform.cs
│   ├── README.md
│   └── TODO.md
│
└── SAMPLE_PROJECTS_OVERVIEW.md       (This file)
```

---

## Next Steps

1. **Review Each Project**: Start with the README.md in each project
2. **Understand Architecture**: Review the csproj files for dependencies
3. **Implement Components**: Use TODO.md as implementation guide
4. **Test**: Build and run each project
5. **Integrate**: Connect to actual OpenCode Serve API
6. **Deploy**: Follow deployment instructions in each project

---

## Additional Resources

- [Microsoft Blazor Documentation](https://docs.microsoft.com/aspnet/core/blazor)
- [MAUI Documentation](https://docs.microsoft.com/dotnet/maui)
- [OpenCode Documentation](./../../README.md)
- Individual project README.md files
- Individual project TODO.md files

---

## Support

For issues or questions:
1. Check the relevant project's README.md
2. Review the TODO.md for implementation guidance
3. Examine the LionFire.OpenCode library documentation
4. Check MAUI or Blazor official documentation
