# Architecture Diagrams

## System Architecture

```
+------------------------------------------------------------------+
|                    Application Layer                              |
|  +------------------------------------------------------------+  |
|  |              User Application / Service                     |  |
|  |  (ASP.NET Core, Console App, Worker Service, etc.)         |  |
|  +------------------------------------------------------------+  |
+------------------------------------------------------------------+
                              |
                              | Uses AIAgent interface
                              v
+------------------------------------------------------------------+
|                Microsoft Agent Framework                          |
|  +------------------------------------------------------------+  |
|  |  WorkflowHostAgent  |  Multi-Agent Orchestration           |  |
|  |  AIAgentBuilder     |  Middleware Pipeline                 |  |
|  +------------------------------------------------------------+  |
|  |                                                             |  |
|  |  +----------------+  +----------------+  +----------------+ |  |
|  |  | AIAgent        |  | AgentThread    |  | AgentRun      | |  |
|  |  | (abstract)     |  | (abstract)     |  | Response      | |  |
|  |  +----------------+  +----------------+  +----------------+ |  |
|  |                                                             |  |
+------------------------------------------------------------------+
                              |
                              | Extends/Implements
                              v
+------------------------------------------------------------------+
|              OpencodeAI.AgentFramework Package                    |
|  +------------------------------------------------------------+  |
|  |                                                             |  |
|  |  +--------------------+     +------------------------+     |  |
|  |  |   OpencodeAgent    |     |  OpencodeAgentThread   |     |  |
|  |  | (extends AIAgent)  |     | (extends AgentThread)  |     |  |
|  |  +--------------------+     +------------------------+     |  |
|  |           |                           |                     |  |
|  |           |                           |                     |  |
|  |  +--------------------+     +------------------------+     |  |
|  |  |  MessageConverter  |     |  ThreadStateManager    |     |  |
|  |  +--------------------+     +------------------------+     |  |
|  |           |                           |                     |  |
|  |  +--------------------+     +------------------------+     |  |
|  |  | OpencodeAgentOpts  |     | ServiceCollection Ext. |     |  |
|  |  +--------------------+     +------------------------+     |  |
|  |                                                             |  |
+------------------------------------------------------------------+
                              |
                              | Wraps/Uses
                              v
+------------------------------------------------------------------+
|                    OpencodeAI Package                             |
|  +------------------------------------------------------------+  |
|  |                                                             |  |
|  |  +------------------+  +-------------------+               |  |
|  |  | IOpencodeClient  |  | OpencodeClient    |               |  |
|  |  +------------------+  +-------------------+               |  |
|  |           |                                                 |  |
|  |  +------------------+  +-------------------+               |  |
|  |  | IConversation    |  | Conversation      |               |  |
|  |  +------------------+  +-------------------+               |  |
|  |           |                                                 |  |
|  |  +------------------+  +-------------------+               |  |
|  |  | Tool Handlers    |  | Streaming Support |               |  |
|  |  +------------------+  +-------------------+               |  |
|  |                                                             |  |
+------------------------------------------------------------------+
                              |
                              | HTTP/HTTPS
                              v
+------------------------------------------------------------------+
|                    OpenCode REST API                              |
+------------------------------------------------------------------+
```

## Component Diagram

```
+---------------------------------------------------------------------+
|                      OpencodeAI.AgentFramework                       |
+---------------------------------------------------------------------+
|                                                                      |
|  +-----------------------+          +-----------------------+       |
|  |    OpencodeAgent      |          | OpencodeAgentThread   |       |
|  |-----------------------|          |-----------------------|       |
|  | - _client             |<>------->| - _threadId           |       |
|  | - _options            |          | - _conversation       |       |
|  | - _id                 |          | - _messages           |       |
|  |-----------------------|          |-----------------------|       |
|  | + GetNewThread()      |          | + Serialize()         |       |
|  | + DeserializeThread() |          | + GetService()        |       |
|  | + RunAsync()          |          +-----------------------+       |
|  | + RunStreamingAsync() |                    ^                     |
|  | + GetService()        |                    |                     |
|  +-----------------------+                    |                     |
|            |                                  |                     |
|            | uses                             | creates             |
|            v                                  |                     |
|  +-----------------------+          +-----------------------+       |
|  |   MessageConverter    |          | ThreadStateManager    |       |
|  |-----------------------|          |-----------------------|       |
|  | (static class)        |          | - _serializerOptions  |       |
|  |-----------------------|          |-----------------------|       |
|  | + ToConversationMsg() |          | + Serialize()         |       |
|  | + ToChatMessage()     |          | + Deserialize()       |       |
|  | + ToConversationMsgs()|          +-----------------------+       |
|  | + ToChatMessages()    |                                          |
|  +-----------------------+                                          |
|                                                                      |
|  +-----------------------+          +-----------------------+       |
|  | OpencodeAgentOptions  |          | OpencodeAgentRunOpts  |       |
|  |-----------------------|          |-----------------------|       |
|  | + Id                  |          | + Model               |       |
|  | + Name                |          | + MaxTokens           |       |
|  | + Description         |          | + Temperature         |       |
|  | + SystemPrompt        |          +-----------------------+       |
|  | + Tools               |                                          |
|  +-----------------------+                                          |
|                                                                      |
+---------------------------------------------------------------------+
```

## Message Conversion Flow

```
Microsoft.Extensions.AI                      OpencodeAI
+-------------------+                        +----------------------+
|   ChatMessage     |                        | ConversationMessage  |
|-------------------|                        |----------------------|
| + Role (ChatRole) |   MessageConverter     | + Role (MessageRole) |
| + Contents        | =====================> | + Content            |
| + AuthorName      |   ToConversationMsg()  | + Id                 |
| + MessageId       |                        | + CreatedAt          |
| + Additional      |                        | + ToolCalls          |
|   Properties      |                        | + ToolCallId         |
+-------------------+                        +----------------------+
        ^                                             |
        |                                             |
        |                                             v
+-------------------+                        +----------------------+
|   ChatMessage     |   MessageConverter     | ConversationMessage  |
|                   | <===================== |                      |
|                   |    ToChatMessage()     |                      |
+-------------------+                        +----------------------+


Role Mapping:
+------------------+     +-------------------+
| ChatRole.System  | <-> | MessageRole.System|
| ChatRole.User    | <-> | MessageRole.User  |
| ChatRole.Asst    | <-> | MessageRole.Asst  |
| ChatRole.Tool    | <-> | MessageRole.Tool  |
+------------------+     +-------------------+
```

## Streaming Data Flow

```
User Request
     |
     | RunStreamingAsync(messages, thread)
     v
+------------------------+
|     OpencodeAgent      |
|------------------------|
| 1. Validate thread     |
| 2. Convert messages    |
| 3. Get/create conv.    |
+------------------------+
     |
     | thread.Conversation.StreamMessageAsync()
     v
+------------------------+
|    OpencodeAI Client   |
|------------------------|
| 1. Send HTTP request   |
| 2. Open SSE stream     |
| 3. Yield chunks        |
+------------------------+
     |
     | IAsyncEnumerable<ConversationChunk>
     v
+------------------------+
|     OpencodeAgent      |
|------------------------|
| For each chunk:        |
|  - Create Update       |
|  - Set AgentId         |
|  - Set Role            |
|  - Add TextContent     |
|  - Yield update        |
+------------------------+
     |
     | IAsyncEnumerable<AgentRunResponseUpdate>
     v
+------------------------+
|    User Application    |
|------------------------|
| await foreach (update) |
| {                      |
|   Console.Write(       |
|     update.Text);      |
| }                      |
+------------------------+
```

## Thread Serialization

```
+-------------------------+     Serialize()     +------------------------+
|  OpencodeAgentThread    | =================> |      JsonElement       |
|-------------------------|                     |------------------------|
| ThreadId: "abc-123"     |                     | {                      |
| Conversation: {...}     |                     |   "threadId": "abc-123"|
| Messages: [             |                     |   "conversationId":... |
|   {User, "Hello"},      |                     |   "options": {...},    |
|   {Asst, "Hi there"}    |                     |   "messages": [...]    |
| ]                       |                     | }                      |
+-------------------------+                     +------------------------+
          ^                                               |
          |                                               |
          |              DeserializeThread()              |
          +-----------------------------------------------+


Persistence Flow:
+----------------+    +----------------+    +----------------+
|    Thread      |--->|  Serialize()   |--->|  JSON String   |
|    Object      |    +----------------+    +----------------+
+----------------+                                 |
                                                   | Save to DB/File
                                                   v
+----------------+    +----------------+    +----------------+
|    Thread      |<---|  Deserialize() |<---|  JSON String   |
|    Object      |    +----------------+    +----------------+
+----------------+                          Load from DB/File
```

## Middleware Pipeline

```
                         Request Flow
                             |
                             v
+--------------------------------------------------------------------+
|                        User Application                             |
+--------------------------------------------------------------------+
                             |
                             v
+--------------------------------------------------------------------+
|  +----------------------------------------------------------+     |
|  |                   Outer Middleware                        |     |
|  |  (Logging, Telemetry, Rate Limiting)                     |     |
|  +----------------------------------------------------------+     |
|                             |                                      |
|                             v                                      |
|  +----------------------------------------------------------+     |
|  |                   Middle Middleware                       |     |
|  |  (Retry, Circuit Breaker, Caching)                       |     |
|  +----------------------------------------------------------+     |
|                             |                                      |
|                             v                                      |
|  +----------------------------------------------------------+     |
|  |                   Inner Middleware                        |     |
|  |  (Approval, Content Filtering)                           |     |
|  +----------------------------------------------------------+     |
|                             |                                      |
+--------------------------------------------------------------------+
                             |
                             v
+--------------------------------------------------------------------+
|                       OpencodeAgent                                 |
|                    (Core Implementation)                            |
+--------------------------------------------------------------------+
                             |
                             v
+--------------------------------------------------------------------+
|                     OpencodeAI Client                               |
+--------------------------------------------------------------------+
                             |
                             v
+--------------------------------------------------------------------+
|                      OpenCode API                                   |
+--------------------------------------------------------------------+


Middleware Implementation Pattern:
+---------------------------------+
|     DelegatingAIAgent           |
|---------------------------------|
| - innerAgent: AIAgent           |
|---------------------------------|
| + RunAsync():                   |
|   - Pre-process                 |
|   - innerAgent.RunAsync()       |
|   - Post-process                |
+---------------------------------+
```

## Dependency Injection Integration

```
+---------------------------------------------------------------------+
|                    IServiceCollection                                |
+---------------------------------------------------------------------+
|                                                                      |
|  services.AddOpencodeClient(options => ...)                         |
|     |                                                                |
|     +---> Registers: IOpencodeClient, OpencodeClient                |
|                                                                      |
|  services.AddOpencodeAgent(options => ...)                          |
|     |                                                                |
|     +---> Requires: IOpencodeClient (from above)                    |
|     +---> Registers: AIAgent, OpencodeAgent                         |
|     +---> Configures: IOptions<OpencodeAgentOptions>                |
|                                                                      |
+---------------------------------------------------------------------+

Resolution Flow:
+------------------+       +---------------------+
| IServiceProvider |------>| Resolve AIAgent     |
+------------------+       +---------------------+
                                    |
                                    v
                          +---------------------+
                          | OpencodeAgent       |
                          |---------------------|
                          | Requires:           |
                          | - IOpencodeClient   |
                          | - IOptions<...>     |
                          +---------------------+
                                    |
              +---------------------+---------------------+
              |                                           |
              v                                           v
    +------------------+                     +------------------------+
    | IOpencodeClient  |                     | IOptions<...>          |
    | (resolved)       |                     | (resolved)             |
    +------------------+                     +------------------------+
```

## Multi-Agent Workflow

```
+------------------------------------------------------------------+
|                      Workflow Orchestrator                        |
|           (WorkflowHostAgent / Custom Orchestration)              |
+------------------------------------------------------------------+
        |                    |                    |
        v                    v                    v
+----------------+  +----------------+  +------------------+
|  Planner Agent |  | OpencodeAgent  |  |  Reviewer Agent  |
|  (OpenAI)      |  | (OpencodeAI)   |  |  (Anthropic)     |
+----------------+  +----------------+  +------------------+
        |                    |                    |
        | "Plan: Create     | "Code: class User  | "Review: Good
        |  user service"    |  { ... }"          |  structure..."
        v                    v                    v
+------------------------------------------------------------------+
|                       Workflow State                              |
|  {                                                                |
|    "plan": "...",                                                 |
|    "code": "...",                                                 |
|    "review": "..."                                                |
|  }                                                                |
+------------------------------------------------------------------+


Sequential Flow:
+----------+     +----------+     +----------+     +----------+
| Planner  |---->|  Coder   |---->| Reviewer |---->|  Output  |
| (Plan)   |     | (Code)   |     | (Review) |     | (Result) |
+----------+     +----------+     +----------+     +----------+
                      ^
                      |
                 OpencodeAgent
```
