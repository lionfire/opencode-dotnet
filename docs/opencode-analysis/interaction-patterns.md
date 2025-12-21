# OpenCode Web UI Interaction Patterns

**Source**: `/dv/opencode/packages/ui/`
**Framework**: SolidJS with @kobalte/core

## Component Library: @kobalte/core

OpenCode uses **@kobalte/core** for accessible UI primitives. This is similar to:
- Radix UI (React)
- Headless UI (React/Vue)

Kobalte provides:
- Keyboard navigation
- ARIA attributes
- Focus management
- State management (open/closed)

## Interactive Component Patterns

### 1. Collapsible Pattern

Used for expandable/collapsible content sections.

**Component Structure**:
```tsx
<Collapsible variant="normal|ghost" open={isOpen} onOpenChange={setIsOpen}>
  <Collapsible.Trigger>
    {/* Clickable header */}
    <Collapsible.Arrow />  {/* Optional indicator */}
  </Collapsible.Trigger>
  <Collapsible.Content>
    {/* Expandable content */}
  </Collapsible.Content>
</Collapsible>
```

**Interactions**:
- Click trigger to toggle open/closed
- Space/Enter on focused trigger toggles
- Arrow animates based on state (via CSS)

**Usage in OpenCode**:
- Tool call details (expand to see output)
- "Show details" / "Hide details" in SessionTurn
- Basic tool expandable content

### 2. Accordion Pattern

Multi-item collapsible with optional single/multiple selection.

**Component Structure**:
```tsx
<Accordion multiple>  {/* or single-select mode */}
  <Accordion.Item value="unique-id">
    <Accordion.Header>
      <Accordion.Trigger>
        {/* Item header content */}
      </Accordion.Trigger>
    </Accordion.Header>
    <Accordion.Content>
      {/* Expandable item content */}
    </Accordion.Content>
  </Accordion.Item>
</Accordion>
```

**Interactions**:
- Click trigger to expand item
- In single mode, expanding one collapses others
- In multiple mode, any combination can be open
- Arrow Up/Down navigates between items
- Home/End jumps to first/last item

**Usage in OpenCode**:
- File diffs in session summary (multiple mode)
- Each file is an accordion item with diff viewer

### 3. Dialog Pattern

Modal dialog with overlay.

**Component Structure**:
```tsx
<Dialog open={isOpen} onOpenChange={setIsOpen} trigger={<Button>Open</Button>}>
  <Dialog.Header>
    <Dialog.Title>Title</Dialog.Title>
    <Dialog.CloseButton />
  </Dialog.Header>
  <Dialog.Body>
    {/* Dialog content */}
  </Dialog.Body>
</Dialog>
```

**Interactions**:
- Click trigger opens dialog
- Escape closes dialog
- Click overlay closes dialog
- Focus trapped inside dialog
- Focus returns to trigger on close

**Usage in OpenCode**:
- Model/provider selection dialog
- Confirmation dialogs
- Settings panels

### 4. Dropdown Menu Pattern

Context menu or action menu.

**Component Structure**:
```tsx
<DropdownMenu>
  <DropdownMenu.Trigger>
    <Button>Menu</Button>
  </DropdownMenu.Trigger>
  <DropdownMenu.Portal>
    <DropdownMenu.Content>
      <DropdownMenu.Item onSelect={handleAction}>
        Action
      </DropdownMenu.Item>
      <DropdownMenu.Separator />
      <DropdownMenu.CheckboxItem checked={isChecked}>
        Toggle Option
      </DropdownMenu.CheckboxItem>
      <DropdownMenu.Sub>
        <DropdownMenu.SubTrigger>More...</DropdownMenu.SubTrigger>
        <DropdownMenu.SubContent>
          {/* Submenu items */}
        </DropdownMenu.SubContent>
      </DropdownMenu.Sub>
    </DropdownMenu.Content>
  </DropdownMenu.Portal>
</DropdownMenu>
```

**Interactions**:
- Click trigger opens menu
- Arrow Down focuses first item
- Arrow Up/Down navigates items
- Arrow Right opens submenu
- Arrow Left closes submenu
- Enter/Space selects item
- Escape closes menu
- Click outside closes menu

**Features**:
- CheckboxItem - toggleable items
- RadioGroup/RadioItem - single selection groups
- SubMenu - nested menus

### 5. Select Pattern

Dropdown select for single value selection.

**Component Structure**:
```tsx
<Select
  options={items}
  current={selectedItem}
  value={(item) => item.id}
  label={(item) => item.name}
  groupBy={(item) => item.category}
  onSelect={(item) => setSelected(item)}
  placeholder="Select..."
>
  {(item) => <CustomItemDisplay item={item} />}
</Select>
```

**Interactions**:
- Click trigger opens dropdown
- Arrow Up/Down navigates options
- Enter selects highlighted option
- Escape closes without selecting
- Type to filter (if implemented)

### 6. List Pattern

Virtual list with keyboard navigation and filtering.

**Component Structure**:
```tsx
<List
  items={allItems}
  key={(item) => item.id}
  filterKeys={["name", "description"]}
  current={selectedItem}
  groupBy={(item) => item.category}
  onSelect={(item) => handleSelect(item)}
  emptyMessage="No results"
  filter={searchText}
>
  {(item) => <ItemDisplay item={item} />}
</List>
```

**Interactions**:
- Arrow Up/Down navigates items
- Enter selects active item
- Mouse hover activates item
- Filter text filters list (via parent)
- Scroll follows keyboard navigation
- Groups with headers

**Keyboard State**:
- `data-active` - keyboard focused item
- `data-selected` - currently selected item

### 7. Tabs Pattern

Tab navigation for switching content panels.

**Component Structure**:
```tsx
<Tabs defaultValue="tab1">
  <Tabs.List>
    <Tabs.Trigger value="tab1">Tab 1</Tabs.Trigger>
    <Tabs.Trigger value="tab2">Tab 2</Tabs.Trigger>
    <Tabs.Indicator />  {/* Animated indicator */}
  </Tabs.List>
  <Tabs.Content value="tab1">Content 1</Tabs.Content>
  <Tabs.Content value="tab2">Content 2</Tabs.Content>
</Tabs>
```

**Interactions**:
- Click tab to switch
- Arrow Left/Right navigates tabs
- Home/End jumps to first/last tab
- Tab key moves focus into panel

### 8. Tooltip Pattern

Hover/focus tooltip for additional info.

**Component Structure**:
```tsx
<Tooltip>
  <Tooltip.Trigger>
    <Button>Hover me</Button>
  </Tooltip.Trigger>
  <Tooltip.Portal>
    <Tooltip.Content>
      Tooltip text
      <Tooltip.Arrow />
    </Tooltip.Content>
  </Tooltip.Portal>
</Tooltip>
```

**Interactions**:
- Mouse hover shows tooltip (with delay)
- Focus shows tooltip
- Mouse leave hides tooltip
- Escape hides tooltip

### 9. Checkbox Pattern

Toggle checkbox with label.

**Component Structure**:
```tsx
<Checkbox checked={isChecked} onChange={setChecked} readOnly={false}>
  <label>Checkbox label</label>
</Checkbox>
```

**Interactions**:
- Click to toggle
- Space to toggle when focused

## Animation Patterns

### Typewriter Animation

Character-by-character text reveal.

**Implementation**:
```typescript
const Typewriter = (props) => {
  // Varies timing for natural feel:
  // - 5% chance: 150-250ms (pause)
  // - 15% chance: 80-140ms (slow)
  // - 80% chance: 30-80ms (normal)

  const getTypingDelay = () => {
    const random = Math.random()
    if (random < 0.05) return 150 + Math.random() * 100  // Pause
    if (random < 0.15) return 80 + Math.random() * 60    // Slow
    return 30 + Math.random() * 50                        // Normal
  }

  // Shows cursor while typing, hides 2s after completion
  // Cursor blinks when not typing
}
```

**Usage**:
- Session title reveal
- Summary text reveal

### Staggered Fade-Up

Sequential element appearance animation.

**CSS**:
```css
.fade-up-text {
  animation: fadeUp 0.4s ease-out forwards;
  opacity: 0;
}

@keyframes fadeUp {
  from { opacity: 0; transform: translateY(5px); }
  to { opacity: 1; transform: translateY(0); }
}

/* Staggered delays */
.fade-up-text:nth-child(1) { animation-delay: 0.1s; }
.fade-up-text:nth-child(2) { animation-delay: 0.2s; }
/* ... up to 30 children */
```

### Collapsible Animation

Height transition for expand/collapse.

**CSS** (from collapsible.css):
```css
[data-slot="collapsible-content"] {
  overflow: hidden;
  animation: slideUp 200ms ease-out;
}

[data-slot="collapsible-content"][data-expanded] {
  animation: slideDown 200ms ease-out;
}
```

### Arrow Rotation

Direction indicator animation.

**CSS**:
```css
[data-slot="collapsible-arrow"] svg {
  transition: transform 200ms ease;
}

[data-state="open"] [data-slot="collapsible-arrow"] svg {
  transform: rotate(180deg);
}
```

## Focus Management

### Focus Trapping

Dialog uses Kobalte's built-in focus trapping:
- Tab cycles through focusable elements inside dialog
- Shift+Tab cycles backwards
- Focus cannot escape to background

### Focus Restoration

When dialog closes:
- Focus returns to the element that opened it
- Handled automatically by Kobalte

### Focus Indicators

Focus visible styles:
```css
:focus-visible {
  outline: 2px solid var(--border-selected);
  outline-offset: 2px;
}

/* Or shadow-based */
:focus-visible {
  box-shadow: var(--shadow-xs-border-focus);
}
```

## Mouse vs Keyboard Detection

List component tracks input mode:

```typescript
const [store, setStore] = createStore({
  mouseActive: false,
})

// On mouse move over item:
onMouseMove={() => {
  setStore("mouseActive", true)
  setActive(props.key(item))
}}

// On keyboard navigation:
handleKey = (e: KeyboardEvent) => {
  setStore("mouseActive", false)
  // ...
}

// Scroll behavior differs:
createEffect(() => {
  if (store.mouseActive) return  // Don't auto-scroll during mouse use
  // Auto-scroll to active item during keyboard navigation
})
```

## State Attributes

Components expose state via data attributes for CSS:

| Attribute | Values | Purpose |
|-----------|--------|---------|
| `data-state` | `open`, `closed` | Collapsible/accordion state |
| `data-expanded` | present/absent | Content expansion state |
| `data-active` | `true`, `false` | Keyboard-focused item |
| `data-selected` | `true`, `false` | Currently selected item |
| `data-disabled` | `true`, `false` | Disabled state |
| `data-variant` | `normal`, `ghost` | Style variant |

## Blazor Replication Notes

### Native HTML Equivalents

For simpler components, native HTML may suffice:
- `<details>` + `<summary>` for basic collapsible
- `<dialog>` for modal dialogs
- Custom JS for focus trapping

### Component Libraries

Consider for Blazor:
- **MudBlazor** - comprehensive component library
- **Radzen Blazor** - another option
- **Custom components** with JavaScript interop

### Key Patterns to Replicate

1. **Keyboard navigation** - Arrow keys, Enter, Escape
2. **Focus management** - Trapping, restoration
3. **State exposure** - data-* attributes for CSS
4. **Animations** - CSS transitions, keyframes
5. **ARIA attributes** - Accessibility

### SignalR for Real-time

Blazor Server's SignalR can replace SolidJS reactivity:
- Server pushes updates to UI
- StateHasChanged() triggers re-render
- Similar reactive update pattern
